using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class PrestamoService : IPrestamoService
{
    private readonly IUnitOfWork _unitOfWork;

    public PrestamoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Prestamo>> GetAllAsync()
    {
        return await _unitOfWork.Prestamos.GetAllAsync();
    }

    public async Task<Prestamo?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Prestamos.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosByEstudianteAsync(int estudianteId)
    {
        return await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(estudianteId);
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosByLibroAsync(int libroId)
    {
        return await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libroId);
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosActivosAsync()
    {
        return await _unitOfWork.Prestamos.GetPrestamosActivosAsync();
    }

    public async Task<bool> CreateAsync(Prestamo prestamo)
    {
        try
        {
            await _unitOfWork.Prestamos.AddAsync(prestamo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Prestamo prestamo)
    {
        try
        {
            _unitOfWork.Prestamos.Update(prestamo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
            if (prestamo == null)
                return false;

            _unitOfWork.Prestamos.Remove(prestamo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}