using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibroManager.Repositories;

public class UbicacionRepository : GenericRepository<Ubicacion>, IUbicacionRepository
{
    public UbicacionRepository(ApplicationDbContext context) : base(context)
    {
    }
    
    public override async Task<IEnumerable<Ubicacion>> GetAllAsync()
    {
        return await _context.Ubicaciones
            .Include(u => u.Libros)
            .ToListAsync();
    }
    
    public override async Task<Ubicacion?> GetByIdAsync(int id)
    {
        return await _context.Ubicaciones
            .Include(u => u.Libros)
            .FirstOrDefaultAsync(u => u.UbicacionId == id);
    }
    
    public override async Task<IEnumerable<Ubicacion>> FindAsync(Expression<Func<Ubicacion, bool>> expression)
    {
        return await _context.Ubicaciones
            .Include(u => u.Libros)
            .Where(expression)
            .ToListAsync();
    }
    
    public async Task<bool> HasLibrosAsync(int ubicacionId)
    {
        return await _context.Libros.AnyAsync(l => l.UbicacionId == ubicacionId);
    }
    
    public async Task<Ubicacion?> GetUbicacionWithLibrosAsync(int id)
    {
        return await _context.Ubicaciones
            .Include(u => u.Libros)
            .FirstOrDefaultAsync(u => u.UbicacionId == id);
    }
}