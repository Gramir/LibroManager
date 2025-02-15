using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Repositories;

public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Categoria?> GetByNombreAsync(string nombre)
    {
        return await _context.Set<Categoria>()
            .FirstOrDefaultAsync(c => c.Nombre == nombre);
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasWithLibrosAsync()
    {
        return await _context.Set<Categoria>()
            .Include(c => c.Libros)
            .ToListAsync();
    }
}