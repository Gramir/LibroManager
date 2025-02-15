using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class EstudianteService : IEstudianteService
{
    private readonly IUnitOfWork _unitOfWork;

    public EstudianteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Estudiante>> GetAllAsync()
    {
        return await _unitOfWork.Estudiantes.GetAllAsync();
    }

    public async Task<Estudiante?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Estudiantes.GetByIdAsync(id);
    }

    public async Task<Estudiante?> GetByEmailAsync(string email)
    {
        return await _unitOfWork.Estudiantes.GetByEmailAsync(email);
    }

    public async Task<IEnumerable<Estudiante>> GetEstudiantesWithPrestamosActivosAsync()
    {
        return await _unitOfWork.Estudiantes.GetEstudiantesWithPrestamosActivosAsync();
    }

    public async Task<bool> CreateAsync(Estudiante estudiante)
    {
        try
        {
            if (await _unitOfWork.Estudiantes.GetByEmailAsync(estudiante.Email) != null)
                return false;

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
            var existingEstudiante = await _unitOfWork.Estudiantes.GetByEmailAsync(estudiante.Email);
            if (existingEstudiante != null && existingEstudiante.EstudianteId != estudiante.EstudianteId)
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

            _unitOfWork.Estudiantes.Remove(estudiante);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}