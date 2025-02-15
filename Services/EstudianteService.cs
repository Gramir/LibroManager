using System.Text.RegularExpressions;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class EstudianteService : IEstudianteService
{
    private readonly IUnitOfWork _unitOfWork;
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public EstudianteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Estudiante>> GetAllAsync()
    {
        try
        {
            return await _unitOfWork.Estudiantes.GetAllAsync();
        }
        catch
        {
            return Enumerable.Empty<Estudiante>();
        }
    }

    public async Task<Estudiante?> GetByIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Estudiantes.GetByIdAsync(id);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Estudiante?> GetByEmailAsync(string email)
    {
        try
        {
            if (!IsValidEmail(email))
                return null;

            return await _unitOfWork.Estudiantes.GetByEmailAsync(email);
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<Estudiante>> GetEstudiantesWithPrestamosActivosAsync()
    {
        try
        {
            return await _unitOfWork.Estudiantes.GetEstudiantesWithPrestamosActivosAsync();
        }
        catch
        {
            return Enumerable.Empty<Estudiante>();
        }
    }

    public async Task<bool> CreateAsync(Estudiante estudiante)
    {
        try
        {
            if (!ValidateEstudianteData(estudiante))
                return false;

            var existingEstudiante = await _unitOfWork.Estudiantes.GetByEmailAsync(estudiante.Email);
            if (existingEstudiante != null)
                return false;

            if (estudiante.FechaInscripcion == default)
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

    public async Task<bool> UpdateAsync(Estudiante estudiante)
    {
        try
        {
            if (!ValidateEstudianteData(estudiante))
                return false;

            var estudianteActual = await _unitOfWork.Estudiantes.GetByIdAsync(estudiante.EstudianteId);
            if (estudianteActual == null)
                return false;

            var estudianteConEmail = await _unitOfWork.Estudiantes.GetByEmailAsync(estudiante.Email);
            if (estudianteConEmail != null && estudianteConEmail.EstudianteId != estudiante.EstudianteId)
                return false;

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

    private bool ValidateEstudianteData(Estudiante estudiante)
    {
        if (estudiante == null)
            return false;

        if (string.IsNullOrWhiteSpace(estudiante.Nombre) || estudiante.Nombre.Length > 100)
            return false;

        if (!IsValidEmail(estudiante.Email))
            return false;

        if (estudiante.FechaInscripcion > DateTime.Now)
            return false;

        return true;
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return EmailRegex.IsMatch(email);
    }
}