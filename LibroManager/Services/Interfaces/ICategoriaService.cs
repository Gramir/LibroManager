using LibroManager.DTOs;

namespace LibroManager.Services.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDTO>> GetAllAsync();
    Task<CategoriaDTO?> GetByIdAsync(int id);
    Task<CategoriaDTO?> GetByNombreAsync(string nombre);
    Task<IEnumerable<CategoriaDTO>> GetCategoriasWithLibrosAsync();
    Task<bool> CreateAsync(CategoriaCreateDTO categoria);
    Task<bool> UpdateAsync(CategoriaUpdateDTO categoria);
    Task<bool> DeleteAsync(int id);
}