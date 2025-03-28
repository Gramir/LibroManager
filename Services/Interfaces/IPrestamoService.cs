using LibroManager.DTOs;
using LibroManager.Models;

namespace LibroManager.Services.Interfaces;

public interface IPrestamoService
{
    Task<IEnumerable<PrestamoDTO>> GetAllAsync();
    Task<PrestamoDTO?> GetByIdAsync(int id);
    Task<IEnumerable<PrestamoDTO>> GetPrestamosByEstudianteAsync(int estudianteId);
    Task<IEnumerable<PrestamoDTO>> GetPrestamosByLibroAsync(int libroId);
    Task<IEnumerable<PrestamoDTO>> GetPrestamosActivosAsync();
    Task<bool> CreateAsync(PrestamoCreateDTO prestamo);
    Task<bool> UpdateAsync(PrestamoUpdateDTO prestamo);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteHistorialPrestamosLibroAsync(int libroId);
}