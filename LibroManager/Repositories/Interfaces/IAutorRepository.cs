using LibroManager.Models;

namespace LibroManager.Repositories.Interfaces;

public interface IAutorRepository : IGenericRepository<Autor>
{
    Task<bool> HasLibrosAsync(int autorId);
    Task<Autor?> GetAutorWithLibrosAsync(int id);
}