using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Services;

public class LibroValidationService : ILibroValidationService
{
    private readonly ApplicationDbContext _context;

    public LibroValidationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> LibroEsValido(Libro libro)
    {
        if (libro.AutorId <= 0 || libro.CategoriaId <= 0)
            return false;

        var autorExiste = await _context.Autores.AnyAsync(a => a.AutorId == libro.AutorId);
        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.CategoriaId == libro.CategoriaId);

        return autorExiste && categoriaExiste;
    }

    public async Task<bool> PuedeEliminarAutor(int autorId)
    {
        return !await _context.Libros.AnyAsync(l => l.AutorId == autorId);
    }

    public async Task<bool> PuedeEliminarCategoria(int categoriaId)
    {
        return !await _context.Libros.AnyAsync(l => l.CategoriaId == categoriaId);
    }

    public async Task<bool> LibroEstaPrestado(int libroId)
    {
        return await _context.Prestamos
            .AnyAsync(p => p.LibroId == libroId && p.FechaVencimiento > DateTime.Now);
    }

    public bool FechasPrestamoSonValidas(DateTime fechaPrestamo, DateTime fechaVencimiento)
    {
        var hoy = DateTime.Today;
        
        // La fecha de préstamo no puede ser futura
        if (fechaPrestamo.Date > hoy)
            return false;

        // La fecha de vencimiento debe ser posterior a la fecha de préstamo
        // y no puede ser más de 30 días después
        if (fechaVencimiento.Date <= fechaPrestamo.Date || 
            fechaVencimiento.Date > fechaPrestamo.Date.AddDays(30))
            return false;

        return true;
    }

    public async Task<bool> PrestamoEsValido(Prestamo prestamo)
    {
        if (prestamo.LibroId <= 0 || prestamo.EstudianteId <= 0)
            return false;

        if (!FechasPrestamoSonValidas(prestamo.FechaPrestamo, prestamo.FechaVencimiento))
            return false;

        var libroExiste = await _context.Libros.AnyAsync(l => l.LibroId == prestamo.LibroId);
        var estudianteExiste = await _context.Estudiantes.AnyAsync(e => e.EstudianteId == prestamo.EstudianteId);
        var libroPrestado = await LibroEstaPrestado(prestamo.LibroId);

        return libroExiste && estudianteExiste && !libroPrestado;
    }
}