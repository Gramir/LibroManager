using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Repositories;

public class PrestamoRepository : GenericRepository<Prestamo>, IPrestamoRepository
{
    public PrestamoRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Prestamo>> GetAllAsync()
    {
        return await _context.Set<Prestamo>()
            .Include(p => p.Libro)
            .Include(p => p.Estudiante)
            .ToListAsync();
    }

    public override async Task<Prestamo?> GetByIdAsync(int id)
    {
        return await _context.Set<Prestamo>()
            .Include(p => p.Libro)
            .Include(p => p.Estudiante)
            .FirstOrDefaultAsync(p => p.PrestamoId == id);
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosByEstudianteAsync(int estudianteId)
    {
        return await _context.Set<Prestamo>()
            .Include(p => p.Libro)
            .Include(p => p.Estudiante)
            .Where(p => p.EstudianteId == estudianteId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosByLibroAsync(int libroId)
    {
        return await _context.Set<Prestamo>()
            .Include(p => p.Libro)
            .Include(p => p.Estudiante)
            .Where(p => p.LibroId == libroId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Prestamo>> GetPrestamosActivosAsync()
    {
        var now = DateTime.Now;
        return await _context.Set<Prestamo>()
            .Include(p => p.Libro)
            .Include(p => p.Estudiante)
            .Where(p => p.Estado == EstadoPrestamo.Activo && p.FechaVencimiento >= now)
            .ToListAsync();
    }

    public async Task<bool> EliminarHistorialPrestamosAsync(int libroId)
    {
        try
        {
            var prestamos = await GetPrestamosByLibroAsync(libroId);
            var prestamosNoActivos = prestamos.Where(p => p.Estado != EstadoPrestamo.Activo);
            
            foreach (var prestamo in prestamosNoActivos)
            {
                _context.Prestamos.Remove(prestamo);
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}