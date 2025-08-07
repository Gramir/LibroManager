using LibroManager.Models;

namespace LibroManager.Repositories.Interfaces;

public interface ICategoriaRepository : IGenericRepository<Categoria>
{
    Task<Categoria?> GetByNombreAsync(string nombre);
    Task<IEnumerable<Categoria>> GetCategoriasWithLibrosAsync();
}