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

    public async Task<bool> UpdateAsync(PrestamoUpdateDTO prestamoDto)
    {
        try
        {
            var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(prestamoDto.PrestamoId);
            if (prestamo == null)
            {
                _logger.LogWarning("Préstamo no encontrado: {PrestamoId}", prestamoDto.PrestamoId);
                return false;
            }

            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            if (libro == null)
            {
                _logger.LogWarning("Libro no encontrado: {LibroId}", prestamo.LibroId);
                return false;
            }

            // Actualizar los valores del préstamo
            prestamo.FechaDevolucion = prestamoDto.FechaDevolucion;
            prestamo.Estado = prestamoDto.Estado;

            _unitOfWork.Prestamos.Update(prestamo);

            // Si el préstamo ha sido devuelto, actualizar el estado del libro
            if (prestamo.Estado == EstadoPrestamo.Concluido)
            {
                await ActualizarEstadoLibroAsync(libro);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Préstamo actualizado: {PrestamoId}", prestamo.PrestamoId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar préstamo {PrestamoId}", prestamoDto.PrestamoId);
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

    private async Task ActualizarEstadoLibroAsync(Libro libro)
    {
        // Verificar si hay préstamos activos para este libro
        var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libro.LibroId);
        var tieneActivosCount = prestamosActivos.Any(p => p.Estado == EstadoPrestamo.Activo);
        
        if (!tieneActivosCount)
        {
            libro.Estado = EstadoLibro.Disponible;
            _logger.LogInformation("Estado del libro actualizado a Disponible: {LibroId}", libro.LibroId);
        }
    }
}