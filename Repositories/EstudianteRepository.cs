using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Repositories;

public class EstudianteRepository : GenericRepository<Estudiante>, IEstudianteRepository
{
    public EstudianteRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IEnumerable<Estudiante>> GetAllAsync()
    {
        return await _context.Set<Estudiante>()
            .Include(e => e.Prestamos)
            .ToListAsync();
    }

    public override async Task<Estudiante?> GetByIdAsync(int id)
    {
        return await _context.Set<Estudiante>()
            .Include(e => e.Prestamos)
            .FirstOrDefaultAsync(e => e.EstudianteId == id);
    }

    public async Task<Estudiante?> GetByEmailAsync(string email)
    {
        return await _context.Set<Estudiante>()
            .Include(e => e.Prestamos)
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<IEnumerable<Estudiante>> GetEstudiantesWithPrestamosActivosAsync()
    {
        return await _context.Set<Estudiante>()
            .Include(e => e.Prestamos)
            .Where(e => e.Prestamos!.Any(p => p.FechaVencimiento >= DateTime.Now))
            .ToListAsync();
    }
}