using AutoMapper;
using System.Text.RegularExpressions;
using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class EstudianteService : IEstudianteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int MAX_NOMBRE_LENGTH = 100;

    public EstudianteService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EstudianteDTO>> GetAllAsync()
    {
        var estudiantes = await _unitOfWork.Estudiantes.GetAllAsync();
        return _mapper.Map<IEnumerable<EstudianteDTO>>(estudiantes);
    }

    public async Task<EstudianteDTO?> GetByIdAsync(int id)
    {
        var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(id);
        return _mapper.Map<EstudianteDTO>(estudiante);
    }

    public async Task<EstudianteDTO?> GetByEmailAsync(string email)
    {
        try
        {
            var estudiante = await _unitOfWork.Estudiantes.GetByEmailAsync(email);
            return _mapper.Map<EstudianteDTO>(estudiante);
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<EstudianteDTO>> GetEstudiantesWithPrestamosActivosAsync()
    {
        var estudiantes = await _unitOfWork.Estudiantes.GetEstudiantesWithPrestamosActivosAsync();
        return _mapper.Map<IEnumerable<EstudianteDTO>>(estudiantes);
    }

    public async Task<bool> CreateAsync(EstudianteCreateDTO estudianteDto)
    {
        try
        {
            if (!ValidateEstudianteData(estudianteDto))
                return false;

            var existingEstudiante = await _unitOfWork.Estudiantes.GetByEmailAsync(estudianteDto.Email);
            if (existingEstudiante != null)
                return false;

            var estudiante = _mapper.Map<Estudiante>(estudianteDto);
            estudiante.FechaInscripcion = DateTime.Now;

            await _unitOfWork.Estudiantes.AddAsync(estudiante);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(EstudianteUpdateDTO estudianteDto)
    {
        try
        {
            if (!ValidateEstudianteData(estudianteDto))
                return false;

            var existingEstudiante = await _unitOfWork.Estudiantes.GetByIdAsync(estudianteDto.EstudianteId);
            if (existingEstudiante == null)
                return false;

            var estudiante = _mapper.Map<Estudiante>(estudianteDto);
            estudiante.FechaInscripcion = existingEstudiante.FechaInscripcion;

            _unitOfWork.Estudiantes.Update(estudiante);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(id);
            if (estudiante == null)
                return false;

            var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(id);
            if (prestamosActivos.Any(p => p.FechaVencimiento >= DateTime.Now))
                return false;

            _unitOfWork.Estudiantes.Remove(estudiante);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool ValidateEstudianteData(EstudianteCreateDTO estudiante)
    {
        if (string.IsNullOrWhiteSpace(estudiante.Nombre) || estudiante.Nombre.Length > MAX_NOMBRE_LENGTH)
            return false;

        if (string.IsNullOrWhiteSpace(estudiante.Email) || !IsValidEmail(estudiante.Email))
            return false;

        return true;
    }

    private bool IsValidEmail(string email)
    {
        var pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }
}