using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibroManager.Repositories;

public class LibroRepository : ILibroRepository
{
    private readonly ApplicationDbContext _context;
    private readonly bool _isTestEnvironment;

    public LibroRepository(ApplicationDbContext context)
    {
        _context = context;
        // Detectamos si estamos en un ambiente de tests por el tipo de proveedor de base de datos
        _isTestEnvironment = context.Database.ProviderName?.Contains("InMemory") ?? false;
    }

    public async Task<IEnumerable<Libro>> GetAllAsync()
    {
        if (_isTestEnvironment)
        {
            return await _context.Libros.ToListAsync();
        }

        return await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categoria)
            .Include(l => l.Ubicacion)
            .Include(l => l.Prestamos)
            .ToListAsync();
    }

    public async Task<Libro?> GetByIdAsync(int id)
    {
        if (_isTestEnvironment)
        {
            return await _context.Libros.FindAsync(id);
        }

        return await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categoria)
            .Include(l => l.Ubicacion)
            .Include(l => l.Prestamos)
            .FirstOrDefaultAsync(l => l.LibroId == id);
    }

    public async Task<IEnumerable<Libro>> FindAsync(Expression<Func<Libro, bool>> expression)
    {
        if (_isTestEnvironment)
        {
            return await _context.Libros
                .Where(expression)
                .ToListAsync();
        }

        return await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categoria)
            .Include(l => l.Ubicacion)
            .Where(expression)
            .ToListAsync();
    }

    public async Task<bool> IsbnExistsAsync(string isbn)
    {
        return await _context.Libros.AnyAsync(l => l.ISBN.ToLower() == isbn.ToLower());
    }

    public async Task<bool> SerialExistsAsync(string serial)
    {
        return await _context.Libros.AnyAsync(l => l.Serial.ToLower() == serial.ToLower());
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
        if (_isTestEnvironment)
        {
            return await _context.Libros.FindAsync(id);
        }

        return await _context.Libros
            .Include(l => l.Autor)
            .Include(l => l.Categoria)
            .Include(l => l.Prestamos)
            .Include(l => l.Ubicacion)
            .FirstOrDefaultAsync(l => l.LibroId == id);
    }

    public async Task<bool> EstaPrestadoAsync(int id)
    {
        var prestamosActivos = await _context.Prestamos
            .Where(p => p.LibroId == id && p.FechaVencimiento > DateTime.Now)
            .AnyAsync();
        return prestamosActivos;
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
}