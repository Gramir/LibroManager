using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Services;

public class LibroValidationService(ApplicationDbContext context) : ILibroValidationService
{
    private readonly ApplicationDbContext _context = context;

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
            .AnyAsync(p => p.LibroId == libroId && p.Estado == EstadoPrestamo.Activo);
    }

    public bool FechasPrestamoSonValidas(DateTime fechaPrestamo, DateTime fechaVencimiento)
    {
        return fechaVencimiento > fechaPrestamo;
    }

    public async Task<bool> PrestamoEsValido(Prestamo prestamo)
    {
        if (prestamo.LibroId <= 0 || prestamo.EstudianteId <= 0)
            return false;

        var libro = await _context.Libros.FindAsync(prestamo.LibroId);
        var estudiante = await _context.Estudiantes.FindAsync(prestamo.EstudianteId);

        return libro != null && estudiante != null;
    }

    public async Task<bool> ValidarUbicacionSeleccionada(string ubicacionString)
    {
        if (string.IsNullOrEmpty(ubicacionString))
            return false;

        var partes = ubicacionString.Split('-');
        if (partes.Length != 3)
            return false;

        var ubicaciones = await _context.Ubicaciones.ToListAsync();
        return ubicaciones.Any(u => u.ObtenerUbicacionFormateada() == ubicacionString);
    }
}