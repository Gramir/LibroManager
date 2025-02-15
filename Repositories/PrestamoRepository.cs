using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Repositories;

public class PrestamoRepository : GenericRepository<Prestamo>, IPrestamoRepository
{
    public PrestamoRepository(ApplicationDbContext context) : base(context) { }

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
        return await _context.Set<Prestamo>()
            .Include(p => p.Libro)
            .Include(p => p.Estudiante)
            .Where(p => p.FechaVencimiento >= DateTime.Now)
            .ToListAsync();
    }
}