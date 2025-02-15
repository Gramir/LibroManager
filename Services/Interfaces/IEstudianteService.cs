using LibroManager.Models;

namespace LibroManager.Services.Interfaces;

public interface IEstudianteService
{
    Task<IEnumerable<Estudiante>> GetAllAsync();
    Task<Estudiante?> GetByIdAsync(int id);
    Task<Estudiante?> GetByEmailAsync(string email);
    Task<IEnumerable<Estudiante>> GetEstudiantesWithPrestamosActivosAsync();
    Task<bool> CreateAsync(Estudiante estudiante);
    Task<bool> UpdateAsync(Estudiante estudiante);
    Task<bool> DeleteAsync(int id);
}