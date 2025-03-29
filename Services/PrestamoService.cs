using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class PrestamoService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILibroValidationService libroValidationService,
    ILogger<PrestamoService> logger) : IPrestamoService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILibroValidationService _libroValidationService = libroValidationService;
    private readonly ILogger<PrestamoService> _logger = logger;

    private async Task VerificarYActualizarEstadoPrestamo(Prestamo prestamo)
    {
        // Si tiene fecha de devolución, está concluido
        if (prestamo.FechaDevolucion.HasValue && prestamo.Estado != EstadoPrestamo.Concluido)
        {
            prestamo.Estado = EstadoPrestamo.Concluido;
            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            if (libro != null)
            {
                libro.Estado = EstadoLibro.Disponible;
                _unitOfWork.Libros.Update(libro);
            }
            _unitOfWork.Prestamos.Update(prestamo);
            await _unitOfWork.SaveChangesAsync();
        }
        // Si no tiene fecha de devolución y está vencido, marcar como expirado y libro como perdido
        else if (!prestamo.FechaDevolucion.HasValue && prestamo.FechaVencimiento < DateTime.Now)
        {
            // Solo actualizar si no estaba ya expirado o el libro no estaba perdido
            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            if (libro != null && (prestamo.Estado != EstadoPrestamo.Expirado || libro.Estado != EstadoLibro.Perdido))
            {
                prestamo.Estado = EstadoPrestamo.Expirado;
                libro.Estado = EstadoLibro.Perdido;
                _unitOfWork.Libros.Update(libro);
                _unitOfWork.Prestamos.Update(prestamo);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Préstamo marcado como expirado y libro como perdido: Préstamo {PrestamoId}, Libro {LibroId}",
                    prestamo.PrestamoId,
                    prestamo.LibroId
                );
            }
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetAllAsync()
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetAllAsync();

            // Verificar y actualizar el estado de cada préstamo antes de mapearlos
            foreach (var prestamo in prestamos)
            {
                await VerificarYActualizarEstadoPrestamo(prestamo);
            }

            var prestamosDto = _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);

            if (!prestamosDto.Any())
            {
                _logger.LogInformation("No se encontraron préstamos");
            }
            return prestamosDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los préstamos");
            return [];
        }
    }

    public async Task<PrestamoDTO?> GetByIdAsync(int id)
    {
        try
        {
            var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
            if (prestamo != null)
            {
                await VerificarYActualizarEstadoPrestamo(prestamo);
            }
            return _mapper.Map<PrestamoDTO>(prestamo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener préstamo con ID {PrestamoId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosByEstudianteAsync(int estudianteId)
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(estudianteId);
            var prestamosDto = _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);

            // Solo después del mapeo verificamos si la colección está vacía
            if (!prestamosDto.Any())
            {
                _logger.LogInformation("No se encontraron préstamos para el estudiante {EstudianteId}", estudianteId);
            }
            return prestamosDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener préstamos del estudiante {EstudianteId}", estudianteId);
            return [];
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosByLibroAsync(int libroId)
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libroId);
            var prestamosDto = _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);

            // Solo después del mapeo verificamos si la colección está vacía
            if (!prestamosDto.Any())
            {
                _logger.LogInformation("No se encontraron préstamos para el libro {LibroId}", libroId);
            }
            return prestamosDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener préstamos del libro {LibroId}", libroId);
            return [];
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosActivosAsync()
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosActivosAsync();
            foreach (var prestamo in prestamos)
            {
                await VerificarYActualizarEstadoPrestamo(prestamo);
            }
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener préstamos activos");
            return [];
        }
    }

    public async Task<bool> CreateAsync(PrestamoCreateDTO prestamoDto)
    {
        try
        {
            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamoDto.LibroId);
            if (libro == null)
            {
                _logger.LogWarning("Libro no encontrado: {LibroId}", prestamoDto.LibroId);
                return false;
            }

            if (libro.Estado != EstadoLibro.Disponible)
            {
                _logger.LogWarning("El libro {LibroId} no está disponible", prestamoDto.LibroId);
                return false;
            }

            var prestamo = _mapper.Map<Prestamo>(prestamoDto);
            if (!await _libroValidationService.PrestamoEsValido(prestamo))
            {
                _logger.LogWarning("Datos de préstamo no válidos");
                return false;
            }

            if (!_libroValidationService.FechasPrestamoSonValidas(prestamo.FechaPrestamo, prestamo.FechaVencimiento))
            {
                _logger.LogWarning("Fechas de préstamo no válidas");
                return false;
            }

            prestamo.Estado = EstadoPrestamo.Activo;
            libro.Estado = EstadoLibro.Prestado;

            // Actualizar el estado del libro en la base de datos
            _unitOfWork.Libros.Update(libro);

            await _unitOfWork.Prestamos.AddAsync(prestamo);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Préstamo creado: {PrestamoId}, Libro: {LibroId}, Estudiante: {EstudianteId}",
                prestamo.PrestamoId, prestamo.LibroId, prestamo.EstudianteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear préstamo");
            return false;
        }
    }

    public async Task<bool> UpdateAsync(PrestamoUpdateDTO prestamoUpdateDto)
    {
        try
        {
            var prestamoExistente = await _unitOfWork.Prestamos.GetByIdAsync(prestamoUpdateDto.PrestamoId);
            if (prestamoExistente == null)
            {
                _logger.LogWarning("Préstamo no encontrado: {PrestamoId}", prestamoUpdateDto.PrestamoId);
                return false;
            }

            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamoExistente.LibroId);
            if (libro == null)
            {
                _logger.LogWarning("Libro no encontrado: {LibroId}", prestamoExistente.LibroId);
                return false;
            }

            // Actualizar las propiedades del préstamo existente
            prestamoExistente.FechaVencimiento = prestamoUpdateDto.FechaVencimiento;
            prestamoExistente.FechaDevolucion = prestamoUpdateDto.FechaDevolucion;

            // Determinar el estado del préstamo y libro
            if (prestamoExistente.FechaDevolucion.HasValue)
            {
                // Si hay fecha de devolución, el préstamo está concluido
                prestamoExistente.Estado = EstadoPrestamo.Concluido;
                libro.Estado = EstadoLibro.Disponible;
            }
            else if (prestamoExistente.FechaVencimiento < DateTime.Now)
            {
                // Si no hay devolución y está vencido, el préstamo está expirado
                prestamoExistente.Estado = EstadoPrestamo.Expirado;
                libro.Estado = EstadoLibro.Perdido;
            }
            else
            {
                // En cualquier otro caso, el préstamo está activo
                prestamoExistente.Estado = EstadoPrestamo.Activo;
                libro.Estado = EstadoLibro.Prestado;
            }

            // Actualizar usando el préstamo existente
            _unitOfWork.Prestamos.Update(prestamoExistente);
            _unitOfWork.Libros.Update(libro);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Préstamo actualizado: {PrestamoId}", prestamoExistente.PrestamoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar préstamo {PrestamoId}", prestamoUpdateDto.PrestamoId);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
            if (prestamo == null)
            {
                _logger.LogWarning("Préstamo no encontrado para eliminación: {PrestamoId}", id);
                return false;
            }

            if (prestamo.Estado == EstadoPrestamo.Activo)
            {
                _logger.LogWarning("No se puede eliminar un préstamo activo: {PrestamoId}", id);
                return false;
            }

            _unitOfWork.Prestamos.Remove(prestamo);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Préstamo eliminado: {PrestamoId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar préstamo {PrestamoId}", id);
            return false;
        }
    }

    public async Task<bool> DeletePrestamosByLibroIdAsync(int libroId)
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libroId);
            var prestamosNoActivos = prestamos.Where(p => p.Estado != EstadoPrestamo.Activo).ToList();

            if (!prestamosNoActivos.Any())
                return true;

            foreach (var prestamo in prestamosNoActivos)
            {
                _unitOfWork.Prestamos.Remove(prestamo);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar los préstamos del libro {LibroId}", libroId);
            return false;
        }
    }

    private async Task ActualizarEstadoLibroAsync(Libro libro)
    {
        // Verificar si hay préstamos activos para este libro
        var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libro.LibroId);
        var tieneActivosCount = prestamosActivos.Any(p => p.Estado == EstadoPrestamo.Activo);

        if (!tieneActivosCount)
        {
            libro.Estado = EstadoLibro.Disponible;
            // Actualizar explícitamente el estado del libro en la base de datos
            _unitOfWork.Libros.Update(libro);
            _logger.LogInformation("Estado del libro actualizado a Disponible: {LibroId}", libro.LibroId);
        }
    }

    public async Task<bool> TienePrestamosHistoricosAsync(int libroId)
    {
        try
        {
            return await _unitOfWork.Prestamos.TienePrestamosHistoricosAsync(libroId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar préstamos históricos del libro {LibroId}", libroId);
            return false;
        }
    }
}