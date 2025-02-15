using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibroManager.Repositories;

public class LibroRepository : ILibroRepository
{
    private readonly ApplicationDbContext _context;

    public LibroRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Libro>> GetAllAsync()
    {
        return await _context.Libros.ToListAsync();
    }

    public async Task<Libro?> GetByIdAsync(int id)
    {
        return await _context.Libros.FindAsync(id);
    }

    public async Task<IEnumerable<Libro>> FindAsync(Expression<Func<Libro, bool>> expression)
    {
        return await _context.Libros.Where(expression).ToListAsync();
    }

    public async Task AddAsync(Libro libro)
    {
        await _context.Libros.AddAsync(libro);
    }

    public void Update(Libro libro)
    {
        _context.Libros.Update(libro);
    }

    public void Remove(Libro libro)
    {
        _context.Libros.Remove(libro);
    }

    public async Task<bool> IsbnExistsAsync(string isbn)
    {
        return await _context.Libros.AnyAsync(l => l.ISBN == isbn);
    }

    public async Task<IEnumerable<Libro>> GetLibrosWithAutorAndCategoriaAsync()
    {
        return await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categoria)
            .ToListAsync();
    }

    public async Task<Libro?> GetLibroWithDetailsAsync(int id)
    {
        return await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categoria)
            .Include(l => l.Prestamos)
            .FirstOrDefaultAsync(l => l.LibroId == id);
    }

    public async Task<bool> EstaPrestadoAsync(int id)
    {
        var prestamosActivos = await _context.Prestamos
            .Where(p => p.LibroId == id && p.FechaVencimiento > DateTime.Now)
            .AnyAsync();
        return prestamosActivos;
    }
}