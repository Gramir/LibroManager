using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibroManager.Repositories;

public class AutorRepository : IAutorRepository
{
    private readonly ApplicationDbContext _context;

    public AutorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Autor>> GetAllAsync()
    {
        return await _context.Autores
            .Include(a => a.Libros)
            .ToListAsync();
    }

    public async Task<Autor?> GetByIdAsync(int id)
    {
        return await _context.Autores
            .Include(a => a.Libros)
            .FirstOrDefaultAsync(a => a.AutorId == id);
    }

    public async Task<IEnumerable<Autor>> FindAsync(Expression<Func<Autor, bool>> expression)
    {
        return await _context.Autores
            .Include(a => a.Libros)
            .Where(expression)
            .ToListAsync();
    }

    public async Task AddAsync(Autor autor)
    {
        await _context.Autores.AddAsync(autor);
    }

    public void Update(Autor autor)
    {
        _context.Autores.Update(autor);
    }

    public void Remove(Autor autor)
    {
        _context.Autores.Remove(autor);
    }

    public async Task<bool> HasLibrosAsync(int autorId)
    {
        return await _context.Libros.AnyAsync(l => l.AutorId == autorId);
    }

    public async Task<Autor?> GetAutorWithLibrosAsync(int id)
    {
        return await _context.Autores
            .Include(a => a.Libros)
            .FirstOrDefaultAsync(a => a.AutorId == id);
    }
}