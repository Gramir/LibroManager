using Microsoft.EntityFrameworkCore;
using LibroManager.Data.Context;
using LibroManager.Models;

namespace LibroManager.Services
{
    public class LibroValidationService
    {
        private readonly ApplicationDbContext _context;

        public LibroValidationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PuedeEliminarAutor(int autorId)
        {
            return !await _context.Libros.AnyAsync(l => l.AutorId == autorId);
        }

        public async Task<bool> PuedeEliminarCategoria(int categoriaId)
        {
            return !await _context.Libros.AnyAsync(l => l.CategoriaId == categoriaId);
        }

        public async Task<bool> PuedeEliminarEstudiante(int estudianteId)
        {
            return !await _context.Prestamos
                .AnyAsync(p => p.EstudianteId == estudianteId && p.FechaVencimiento >= DateTime.Now);
        }

        public async Task<bool> LibroEstaPrestado(int libroId)
        {
            return await _context.Prestamos
                .AnyAsync(p => p.LibroId == libroId && p.FechaVencimiento >= DateTime.Now);
        }

        public async Task<bool> LibroEsValido(Libro libro)
        {
            if (libro.AutorId <= 0 || libro.CategoriaId <= 0)
                return false;

            var autorExiste = await _context.Autores.AnyAsync(a => a.AutorId == libro.AutorId);
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.CategoriaId == libro.CategoriaId);

            return autorExiste && categoriaExiste;
        }

        public async Task<bool> PrestamoEsValido(Prestamo prestamo)
        {
            if (prestamo.LibroId <= 0 || prestamo.EstudianteId <= 0)
                return false;

            // Verificar que el libro existe y no está prestado
            var libroDisponible = !await LibroEstaPrestado(prestamo.LibroId);
            
            // Verificar que el estudiante existe y está activo
            var estudiante = await _context.Estudiantes.FindAsync(prestamo.EstudianteId);
            var estudianteActivo = estudiante != null;

            return libroDisponible && estudianteActivo;
        }
    }
}