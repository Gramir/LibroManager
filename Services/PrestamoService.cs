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
        try
        {
            return await _unitOfWork.Prestamos.GetAllAsync();
        }
        catch
        {
            return Enumerable.Empty<Prestamo>();
        }
    }

    public async Task<Prestamo?> GetByIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Prestamos.GetByIdAsync(id);
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosByEstudianteAsync(int estudianteId)
    {
        try
        {
            return await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(estudianteId);
        }
        catch
        {
            return Enumerable.Empty<Prestamo>();
        }
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosByLibroAsync(int libroId)
    {
        try
        {
            return await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libroId);
        }
        catch
        {
            return Enumerable.Empty<Prestamo>();
        }
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosActivosAsync()
    {
        try
        {
            return await _unitOfWork.Prestamos.GetPrestamosActivosAsync();
        }
        catch
        {
            return Enumerable.Empty<Prestamo>();
        }
    }

    public async Task<bool> CreateAsync(Prestamo prestamo)
    {
        try
        {
            if (!ValidatePrestamoData(prestamo))
                return false;

            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            if (libro == null)
                return false;

            var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId);
            if (prestamosActivos.Any(p => p.FechaVencimiento >= DateTime.Now))
                return false;

            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(prestamo.EstudianteId);
            if (estudiante == null)
                return false;

            var prestamosEstudiante = await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId);
            if (prestamosEstudiante.Any(p => p.FechaVencimiento < DateTime.Now))
                return false;

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
            if (!ValidatePrestamoData(prestamo))
                return false;

            var existingPrestamo = await _unitOfWork.Prestamos.GetByIdAsync(prestamo.PrestamoId);
            if (existingPrestamo == null)
                return false;

            if (existingPrestamo.LibroId != prestamo.LibroId)
            {
                var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
                if (libro == null)
                    return false;

                var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId);
                if (prestamosActivos.Any(p => p.PrestamoId != prestamo.PrestamoId && p.FechaVencimiento >= DateTime.Now))
                    return false;
            }

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

    private bool ValidatePrestamoData(Prestamo prestamo)
    {
        if (prestamo.FechaPrestamo > prestamo.FechaVencimiento)
            return false;

        if (prestamo.FechaPrestamo > DateTime.Now)
            return false;

        if (prestamo.FechaVencimiento < DateTime.Now)
            return false;

        return true;
    }
}