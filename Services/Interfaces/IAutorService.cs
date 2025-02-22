using LibroManager.DTOs;

namespace LibroManager.Services.Interfaces;

public interface IAutorService
{
    Task<IEnumerable<AutorDTO>> GetAllAsync();
    Task<AutorDTO?> GetByIdAsync(int id);
    Task<bool> CreateAsync(AutorCreateDTO autor);
    Task<bool> UpdateAsync(AutorUpdateDTO autor);
    Task<bool> DeleteAsync(int id);
}