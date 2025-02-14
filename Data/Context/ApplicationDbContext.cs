using Microsoft.EntityFrameworkCore;
using LibroManager.Models;

namespace LibroManager.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Libro> Libros { get; set; }
        public DbSet<Autor> Autores { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de relaciones y borrado en cascada
            modelBuilder.Entity<Libro>()
                .HasOne(l => l.Autor)
                .WithMany(a => a.Libros)
                .OnDelete(DeleteBehavior.Restrict) // Evita el borrado en cascada de libros al eliminar un autor
                .IsRequired();

            modelBuilder.Entity<Libro>()
                .HasOne(l => l.Categoria)
                .WithMany(c => c.Libros)
                .OnDelete(DeleteBehavior.Restrict) // Evita el borrado en cascada de libros al eliminar una categoría
                .IsRequired();

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Libro)
                .WithMany(l => l.Prestamos)
                .OnDelete(DeleteBehavior.Restrict) // Evita el borrado en cascada de préstamos al eliminar un libro
                .IsRequired();

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Estudiante)
                .WithMany(e => e.Prestamos)
                .OnDelete(DeleteBehavior.Restrict) // Evita el borrado en cascada de préstamos al eliminar un estudiante
                .IsRequired();

            // Configuración de valores por defecto
            modelBuilder.Entity<Estudiante>()
                .Property(e => e.FechaInscripcion)
                .HasDefaultValueSql("GETDATE()"); // Establece la fecha actual como valor por defecto

            modelBuilder.Entity<Prestamo>()
                .Property(p => p.FechaPrestamo)
                .HasDefaultValueSql("GETDATE()");

            // Configuración de índices compuestos
            modelBuilder.Entity<Prestamo>()
                .HasIndex(p => new { p.LibroId, p.EstudianteId, p.FechaPrestamo })
                .IsUnique(); // Asegura que no haya préstamos duplicados del mismo libro al mismo estudiante en la misma fecha

            // Configuraciones específicas de columnas
            modelBuilder.Entity<Libro>()
                .Property(l => l.ISBN)
                .IsUnicode(false); // Configura la columna ISBN como VARCHAR en lugar de NVARCHAR para optimizar espacio

            // Validaciones adicionales a nivel de base de datos usando la nueva sintaxis
            modelBuilder.Entity<Prestamo>()
                .ToTable(tb => tb.HasCheckConstraint(
                    "CK_Prestamo_FechaVencimiento",
                    "[FechaVencimiento] > [FechaPrestamo]"
                ));

            modelBuilder.Entity<Libro>()
                .ToTable(tb => tb.HasCheckConstraint(
                    "CK_Libro_ISBN",
                    "LEN([ISBN]) >= 10 AND LEN([ISBN]) <= 13"
                ));
        }
    }
}