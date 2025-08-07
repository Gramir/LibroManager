using LibroManager.DTOs;

namespace LibroManager.Services.Interfaces;

public interface IEstudianteService
{
    Task<IEnumerable<EstudianteDTO>> GetAllAsync();
    Task<EstudianteDTO?> GetByIdAsync(int id);
    Task<EstudianteDTO?> GetByEmailAsync(string email);
    Task<IEnumerable<EstudianteDTO>> GetEstudiantesWithPrestamosActivosAsync();
    Task<bool> CreateAsync(EstudianteCreateDTO estudiante);
    Task<bool> UpdateAsync(EstudianteUpdateDTO estudiante);
    Task<bool> DeleteAsync(int id);
}