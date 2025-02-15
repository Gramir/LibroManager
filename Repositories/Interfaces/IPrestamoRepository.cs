using LibroManager.Models;

namespace LibroManager.Repositories.Interfaces;

public interface IPrestamoRepository : IGenericRepository<Prestamo>
{
    Task<IEnumerable<Prestamo>> GetPrestamosByEstudianteAsync(int estudianteId);
    Task<IEnumerable<Prestamo>> GetPrestamosByLibroAsync(int libroId);
    Task<IEnumerable<Prestamo>> GetPrestamosActivosAsync();
}