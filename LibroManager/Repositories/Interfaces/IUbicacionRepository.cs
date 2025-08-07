using LibroManager.Models;

namespace LibroManager.Repositories.Interfaces;

public interface IUbicacionRepository : IGenericRepository<Ubicacion>
{
    Task<bool> HasLibrosAsync(int ubicacionId);
    Task<Ubicacion?> GetUbicacionWithLibrosAsync(int id);
}