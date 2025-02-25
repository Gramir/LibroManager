using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibroManager.Services;

public class PrestamoService : IPrestamoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILibroValidationService _libroValidationService;
    private readonly ILogger<PrestamoService> _logger;

    public PrestamoService(
        IUnitOfWork unitOfWork, 
        IMapper mapper, 
        ILibroValidationService libroValidationService,
        ILogger<PrestamoService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _libroValidationService = libroValidationService;
        _logger = logger;
    }

    public async Task<IEnumerable<PrestamoDTO>> GetAllAsync()
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetAllAsync();
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los préstamos");
            return Enumerable.Empty<PrestamoDTO>();
        }
    }

    public async Task<PrestamoDTO?> GetByIdAsync(int id)
    {
        try
        {
            var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
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
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener préstamos del estudiante {EstudianteId}", estudianteId);
            return Enumerable.Empty<PrestamoDTO>();
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosByLibroAsync(int libroId)
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libroId);
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener préstamos del libro {LibroId}", libroId);
            return Enumerable.Empty<PrestamoDTO>();
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosActivosAsync()
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosActivosAsync();
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener préstamos activos");
            return Enumerable.Empty<PrestamoDTO>();
        }
    }

    public async Task<bool> CreateAsync(PrestamoCreateDTO prestamoDto)
    {
        try
        {
            var prestamo = _mapper.Map<Prestamo>(prestamoDto);
            prestamo.FechaPrestamo = DateTime.Now;
            prestamo.Estado = EstadoPrestamo.Activo;
            prestamo.FechaCreacion = DateTime.Now;

            // Validate using the validation service
            if (!await _libroValidationService.PrestamoEsValido(prestamo))
            {
                _logger.LogWarning("Datos de préstamo no válidos: {PrestamoData}", 
                    new { prestamo.LibroId, prestamo.EstudianteId, prestamo.FechaPrestamo, prestamo.FechaVencimiento });
                return false;
            }

            // Get libro and estudiante
            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(prestamo.EstudianteId);
            
            if (libro == null || estudiante == null)
            {
                _logger.LogWarning("Libro o estudiante no encontrado: LibroId={LibroId}, EstudianteId={EstudianteId}", 
                    prestamo.LibroId, prestamo.EstudianteId);
                return false;
            }

            // Check for expired prestamos
            var prestamosEstudiante = await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId);
            if (prestamosEstudiante.Any(p => p.Estado == EstadoPrestamo.Expirado))
            {
                _logger.LogWarning("Estudiante {EstudianteId} tiene préstamos expirados", prestamo.EstudianteId);
                return false;
            }

            // Check ejemplares availability
            var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libro.LibroId);
            var prestamosCount = prestamosActivos.Count(p => p.Estado == EstadoPrestamo.Activo);
            
            if (prestamosCount >= libro.NumeroEjemplares)
            {
                _logger.LogWarning("No hay ejemplares disponibles para el libro {LibroId}", libro.LibroId);
                return false;
            }
            
            // Update libro estado only if all ejemplares will be borrowed
            if (prestamosCount + 1 >= libro.NumeroEjemplares)
            {
                libro.Estado = EstadoLibro.Prestado;
                _unitOfWork.Libros.Update(libro);
            }
            
            await _unitOfWork.Prestamos.AddAsync(prestamo);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Préstamo creado: {PrestamoId} para LibroId={LibroId}, EstudianteId={EstudianteId}", 
                prestamo.PrestamoId, prestamo.LibroId, prestamo.EstudianteId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear préstamo");
            return false;
        }
    }

    public async Task<bool> UpdateAsync(PrestamoUpdateDTO prestamoDto)
    {
        try
        {
            var prestamo = _mapper.Map<Prestamo>(prestamoDto);
            
            var existingPrestamo = await _unitOfWork.Prestamos.GetByIdAsync(prestamo.PrestamoId);
            if (existingPrestamo == null)
            {
                _logger.LogWarning("Préstamo no encontrado: {PrestamoId}", prestamo.PrestamoId);
                return false;
            }

            // Mantener los valores originales que no deben cambiar
            prestamo.LibroId = existingPrestamo.LibroId;
            prestamo.EstudianteId = existingPrestamo.EstudianteId;
            prestamo.FechaPrestamo = existingPrestamo.FechaPrestamo;
            prestamo.FechaCreacion = existingPrestamo.FechaCreacion;

            if (!ValidatePrestamoData(prestamo))
            {
                _logger.LogWarning("Datos de préstamo no válidos para actualización: {PrestamoId}", prestamo.PrestamoId);
                return false;
            }

            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            if (libro == null)
            {
                _logger.LogWarning("Libro no encontrado para préstamo: LibroId={LibroId}", prestamo.LibroId);
                return false;
            }

            // Actualizar estado del libro según el estado del préstamo
            if (prestamo.FechaDevolucion.HasValue)
            {
                prestamo.Estado = EstadoPrestamo.Concluido;
                
                // Verificar si hay que actualizar el estado del libro a Disponible
                await ActualizarEstadoLibroAsync(libro);
            }
            else if (prestamo.Estado == EstadoPrestamo.Expirado)
            {
                // Solo marcar como perdido si no hay ejemplares disponibles
                var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libro.LibroId);
                var prestamosCount = prestamosActivos.Count(p => p.Estado == EstadoPrestamo.Activo);
                
                if (prestamosCount >= libro.NumeroEjemplares)
                {
                    libro.Estado = EstadoLibro.Perdido;
                }
            }

            _unitOfWork.Libros.Update(libro);
            _unitOfWork.Prestamos.Update(prestamo);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Préstamo actualizado: {PrestamoId}, Estado={Estado}", 
                prestamo.PrestamoId, prestamo.Estado);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar préstamo");
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

            // Si el préstamo está activo, debemos actualizar el estado del libro
            if (prestamo.Estado == EstadoPrestamo.Activo)
            {
                var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
                if (libro != null)
                {
                    await ActualizarEstadoLibroAsync(libro);
                }
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

    private bool ValidatePrestamoData(Prestamo prestamo)
    {
        // Normalizamos las fechas para comparar solo las fechas sin tiempo
        var fechaPrestamo = prestamo.FechaPrestamo.Date;
        var fechaVencimiento = prestamo.FechaVencimiento.Date;
        var hoy = DateTime.Today;

        if (fechaPrestamo > fechaVencimiento)
        {
            _logger.LogWarning("Fecha de préstamo posterior a fecha de vencimiento");
            return false;
        }

        if (fechaPrestamo > hoy)
        {
            _logger.LogWarning("Fecha de préstamo en el futuro");
            return false;
        }

        // Si la fecha de vencimiento es pasada, marcar como expirado
        if (fechaVencimiento < hoy && prestamo.Estado != EstadoPrestamo.Concluido)
        {
            _logger.LogInformation("Actualizando estado a Expirado porque la fecha de vencimiento pasó");
            prestamo.Estado = EstadoPrestamo.Expirado;
        }

        if (fechaVencimiento > fechaPrestamo.AddDays(30))
        {
            _logger.LogWarning("Período de préstamo mayor a 30 días");
            return false;
        }

        if (prestamo.FechaDevolucion.HasValue)
        {
            if (prestamo.FechaDevolucion.Value < fechaPrestamo)
            {
                _logger.LogWarning("Fecha de devolución anterior a fecha de préstamo");
                return false;
            }
            
            prestamo.Estado = EstadoPrestamo.Concluido;
        }

        return true;
    }

    private async Task ActualizarEstadoLibroAsync(Libro libro)
    {
        // Verificar si hay préstamos activos para este libro
        var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libro.LibroId);
        var countActivos = prestamosActivos.Count(p => p.Estado == EstadoPrestamo.Activo);
        
        // Si no hay préstamos activos o hay ejemplares disponibles
        if (countActivos < libro.NumeroEjemplares)
        {
            libro.Estado = EstadoLibro.Disponible;
            _logger.LogInformation("Estado del libro actualizado a Disponible: {LibroId}", libro.LibroId);
        }
    }
}