using LibroManager.Models;

namespace LibroManager.Services.Interfaces;

public interface IPrestamoService
{
    Task<IEnumerable<Prestamo>> GetAllAsync();
    Task<Prestamo?> GetByIdAsync(int id);
    Task<IEnumerable<Prestamo>> GetPrestamosByEstudianteAsync(int estudianteId);
    Task<IEnumerable<Prestamo>> GetPrestamosByLibroAsync(int libroId);
    Task<IEnumerable<Prestamo>> GetPrestamosActivosAsync();
    Task<bool> CreateAsync(Prestamo prestamo);
    Task<bool> UpdateAsync(Prestamo prestamo);
    Task<bool> DeleteAsync(int id);
}