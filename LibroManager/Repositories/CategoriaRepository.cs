using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Repositories;

public class CategoriaRepository(ApplicationDbContext context) : GenericRepository<Categoria>(context), ICategoriaRepository
{
    public override async Task<IEnumerable<Categoria>> GetAllAsync()
    {
        return await _context.Set<Categoria>()
            .Include(c => c.Libros)
            .ToListAsync();
    }

    public override async Task<Categoria?> GetByIdAsync(int id)
    {
        return await _context.Set<Categoria>()
            .Include(c => c.Libros)
            .FirstOrDefaultAsync(c => c.CategoriaId == id);
    }

    public async Task<Categoria?> GetByNombreAsync(string nombre)
    {
        return await _context.Set<Categoria>()
            .Include(c => c.Libros)
            .FirstOrDefaultAsync(c => c.Nombre == nombre);
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasWithLibrosAsync()
    {
        return await _context.Set<Categoria>()
            .Include(c => c.Libros)
            .ToListAsync();
    }
}