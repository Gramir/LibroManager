using AutoMapper;
using System.Text.RegularExpressions;
using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibroManager.Services;

public class EstudianteService : IEstudianteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<EstudianteService> _logger;
    private const int MAX_NOMBRE_LENGTH = 100;

    public EstudianteService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EstudianteService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<EstudianteDTO>> GetAllAsync()
    {
        try
        {
            var estudiantes = await _unitOfWork.Estudiantes.GetAllAsync();
            return _mapper.Map<IEnumerable<EstudianteDTO>>(estudiantes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los estudiantes");
            return Enumerable.Empty<EstudianteDTO>();
        }
    }

    public async Task<EstudianteDTO?> GetByIdAsync(int id)
    {
        try
        {
            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(id);
            return _mapper.Map<EstudianteDTO>(estudiante);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiante con ID {EstudianteId}", id);
            return null;
        }
    }

    public async Task<EstudianteDTO?> GetByEmailAsync(string email)
    {
        try
        {
            var estudiante = await _unitOfWork.Estudiantes.GetByEmailAsync(email);
            return _mapper.Map<EstudianteDTO>(estudiante);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiante por email: {Email}", email);
            return null;
        }
    }

    public async Task<IEnumerable<EstudianteDTO>> GetEstudiantesWithPrestamosActivosAsync()
    {
        try
        {
            var estudiantes = await _unitOfWork.Estudiantes.GetEstudiantesWithPrestamosActivosAsync();
            return _mapper.Map<IEnumerable<EstudianteDTO>>(estudiantes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estudiantes con préstamos activos");
            return Enumerable.Empty<EstudianteDTO>();
        }
    }

    public async Task<bool> CreateAsync(EstudianteCreateDTO estudianteDto)
    {
        try
        {
            if (!ValidateEstudianteData(estudianteDto))
            {
                _logger.LogWarning("Datos de estudiante no válidos durante la creación");
                return false;
            }

            var existingEstudiante = await _unitOfWork.Estudiantes.GetByEmailAsync(estudianteDto.Email);
            if (existingEstudiante != null)
            {
                _logger.LogWarning("Ya existe un estudiante con el email: {Email}", estudianteDto.Email);
                return false;
            }

            var estudiante = _mapper.Map<Estudiante>(estudianteDto);
            estudiante.FechaInscripcion = DateTime.Now;

            await _unitOfWork.Estudiantes.AddAsync(estudiante);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Estudiante creado: {EstudianteId}, Nombre: {Nombre}, Email: {Email}",
                estudiante.EstudianteId, estudiante.Nombre, estudiante.Email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear estudiante: {Nombre}, {Email}", 
                estudianteDto.Nombre, estudianteDto.Email);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(EstudianteUpdateDTO estudianteDto)
    {
        try
        {
            if (!ValidateEstudianteData(estudianteDto))
            {
                _logger.LogWarning("Datos de estudiante no válidos durante la actualización: {EstudianteId}", 
                    estudianteDto.EstudianteId);
                return false;
            }

            var existingEstudiante = await _unitOfWork.Estudiantes.GetByIdAsync(estudianteDto.EstudianteId);
            if (existingEstudiante == null)
            {
                _logger.LogWarning("Estudiante no encontrado para actualización: {EstudianteId}", 
                    estudianteDto.EstudianteId);
                return false;
            }

            // Verificar si el email ya existe en otro estudiante
            if (existingEstudiante.Email != estudianteDto.Email)
            {
                var existingByEmail = await _unitOfWork.Estudiantes.GetByEmailAsync(estudianteDto.Email);
                if (existingByEmail != null && existingByEmail.EstudianteId != estudianteDto.EstudianteId)
                {
                    _logger.LogWarning("El email {Email} ya está en uso por otro estudiante", estudianteDto.Email);
                    return false;
                }
            }

            // Usar el objeto mapeado directamente
            var estudiante = _mapper.Map<Estudiante>(estudianteDto);
            // Preservar la fecha de inscripción
            estudiante.FechaInscripcion = existingEstudiante.FechaInscripcion;

            _unitOfWork.Estudiantes.Update(estudiante);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Estudiante actualizado: {EstudianteId}, Nombre: {Nombre}, Email: {Email}", 
                estudiante.EstudianteId, estudiante.Nombre, estudiante.Email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estudiante {EstudianteId}", estudianteDto.EstudianteId);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(id);
            if (estudiante == null)
            {
                _logger.LogWarning("Estudiante no encontrado para eliminación: {EstudianteId}", id);
                return false;
            }

            var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(id);
            if (prestamosActivos.Any(p => p.Estado == EstadoPrestamo.Activo))
            {
                _logger.LogWarning("No se puede eliminar estudiante {EstudianteId} porque tiene préstamos activos", id);
                return false;
            }

            _unitOfWork.Estudiantes.Remove(estudiante);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Estudiante eliminado: {EstudianteId}, Nombre: {Nombre}", 
                estudiante.EstudianteId, estudiante.Nombre);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar estudiante {EstudianteId}", id);
            return false;
        }
    }

    private bool ValidateEstudianteData(EstudianteCreateDTO estudiante)
    {
        if (string.IsNullOrWhiteSpace(estudiante.Nombre))
        {
            _logger.LogWarning("Nombre de estudiante vacío");
            return false;
        }
            
        if (estudiante.Nombre.Length > MAX_NOMBRE_LENGTH)
        {
            _logger.LogWarning("Nombre de estudiante excede el límite de {MaxLength} caracteres: {Length}",
                MAX_NOMBRE_LENGTH, estudiante.Nombre.Length);
            return false;
        }

        if (string.IsNullOrWhiteSpace(estudiante.Email))
        {
            _logger.LogWarning("Email de estudiante vacío");
            return false;
        }
            
        if (!IsValidEmail(estudiante.Email))
        {
            _logger.LogWarning("Email de estudiante con formato inválido: {Email}", estudiante.Email);
            return false;
        }

        return true;
    }

    private bool IsValidEmail(string email)
    {
        var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }
}