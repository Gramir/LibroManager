using LibroManager.Models;

namespace LibroManager.Repositories.Interfaces;

public interface IEstudianteRepository : IGenericRepository<Estudiante>
{
    Task<Estudiante?> GetByEmailAsync(string email);
    Task<IEnumerable<Estudiante>> GetEstudiantesWithPrestamosActivosAsync();
}