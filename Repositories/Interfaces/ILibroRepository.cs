using LibroManager.Models;

namespace LibroManager.Repositories.Interfaces;

public interface ILibroRepository : IGenericRepository<Libro>
{
    Task<bool> IsbnExistsAsync(string isbn);
    Task<IEnumerable<Libro>> GetLibrosWithAutorAndCategoriaAsync();
    Task<Libro?> GetLibroWithDetailsAsync(int id);
    Task<bool> EstaPrestadoAsync(int id);
}