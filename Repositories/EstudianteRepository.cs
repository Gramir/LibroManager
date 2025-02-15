using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Repositories;

public class EstudianteRepository : GenericRepository<Estudiante>, IEstudianteRepository
{
    public EstudianteRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Estudiante?> GetByEmailAsync(string email)
    {
        return await _context.Set<Estudiante>()
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