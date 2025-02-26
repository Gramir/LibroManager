using Microsoft.EntityFrameworkCore;
using LibroManager.Models;
using System;
using System.Collections.Generic;

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
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Libro>()
                .HasOne(l => l.Categoria)
                .WithMany(c => c.Libros)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Libro)
                .WithMany(l => l.Prestamos)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Prestamo>()
                .HasOne(p => p.Estudiante)
                .WithMany(e => e.Prestamos)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Configuración de valores por defecto
            modelBuilder.Entity<Estudiante>()
                .Property(e => e.FechaInscripcion)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Prestamo>()
                .Property(p => p.FechaPrestamo)
                .HasDefaultValueSql("GETDATE()");

            // Configuración de índices compuestos
            modelBuilder.Entity<Prestamo>()
                .HasIndex(p => new { p.LibroId, p.EstudianteId, p.FechaPrestamo })
                .IsUnique();

            // Configuraciones específicas de columnas
            modelBuilder.Entity<Libro>()
                .Property(l => l.ISBN)
                .IsUnicode(false);

            // Validaciones adicionales a nivel de base de datos
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
            
            // Datos de prueba
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var fechaPredeterminada = new DateTime(2023, 1, 1);
            
            // Seed Autores
            modelBuilder.Entity<Autor>().HasData(
                new Autor { AutorId = 1, Nombre = "Gabriel García Márquez", FechaCreacion = fechaPredeterminada },
                new Autor { AutorId = 2, Nombre = "J.K. Rowling", FechaCreacion = fechaPredeterminada },
                new Autor { AutorId = 3, Nombre = "Stephen King", FechaCreacion = fechaPredeterminada },
                new Autor { AutorId = 4, Nombre = "Isabel Allende", FechaCreacion = fechaPredeterminada },
                new Autor { AutorId = 5, Nombre = "Haruki Murakami", FechaCreacion = fechaPredeterminada }
            );

            // Seed Categorías
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { CategoriaId = 1, Nombre = "Novela", FechaCreacion = fechaPredeterminada },
                new Categoria { CategoriaId = 2, Nombre = "Fantasía", FechaCreacion = fechaPredeterminada },
                new Categoria { CategoriaId = 3, Nombre = "Terror", FechaCreacion = fechaPredeterminada },
                new Categoria { CategoriaId = 4, Nombre = "Ciencia Ficción", FechaCreacion = fechaPredeterminada },
                new Categoria { CategoriaId = 5, Nombre = "Historia", FechaCreacion = fechaPredeterminada }
            );

            // Seed Libros
            modelBuilder.Entity<Libro>().HasData(
                new Libro { LibroId = 1, Titulo = "Cien años de soledad", ISBN = "9780307474728", AutorId = 1, CategoriaId = 1, NumeroEjemplares = 3, Estado = EstadoLibro.Disponible, FechaCreacion = fechaPredeterminada },
                new Libro { LibroId = 2, Titulo = "Harry Potter y la piedra filosofal", ISBN = "9788478884452", AutorId = 2, CategoriaId = 2, NumeroEjemplares = 5, Estado = EstadoLibro.Disponible, FechaCreacion = fechaPredeterminada },
                new Libro { LibroId = 3, Titulo = "El resplandor", ISBN = "9788497593793", AutorId = 3, CategoriaId = 3, NumeroEjemplares = 2, Estado = EstadoLibro.Disponible, FechaCreacion = fechaPredeterminada },
                new Libro { LibroId = 4, Titulo = "La casa de los espíritus", ISBN = "9780525433477", AutorId = 4, CategoriaId = 1, NumeroEjemplares = 4, Estado = EstadoLibro.Disponible, FechaCreacion = fechaPredeterminada },
                new Libro { LibroId = 5, Titulo = "Tokio blues", ISBN = "9788483835043", AutorId = 5, CategoriaId = 1, NumeroEjemplares = 2, Estado = EstadoLibro.Disponible, FechaCreacion = fechaPredeterminada },
                new Libro { LibroId = 6, Titulo = "El amor en los tiempos del cólera", ISBN = "9780307387264", AutorId = 1, CategoriaId = 1, NumeroEjemplares = 3, Estado = EstadoLibro.Disponible, FechaCreacion = fechaPredeterminada },
                new Libro { LibroId = 7, Titulo = "Harry Potter y la cámara secreta", ISBN = "9788478884957", AutorId = 2, CategoriaId = 2, NumeroEjemplares = 4, Estado = EstadoLibro.Disponible, FechaCreacion = fechaPredeterminada },
                new Libro { LibroId = 8, Titulo = "It", ISBN = "9781501142970", AutorId = 3, CategoriaId = 3, NumeroEjemplares = 2, Estado = EstadoLibro.Disponible, FechaCreacion = fechaPredeterminada }
            );

            // Fechas fijas para préstamos
            var fechaPrestamo1 = new DateTime(2023, 1, 1);
            var fechaVencimiento1 = new DateTime(2023, 1, 15);
            var fechaDevolucion1 = new DateTime(2023, 1, 10);
            
            var fechaPrestamo2 = new DateTime(2023, 1, 5);
            var fechaVencimiento2 = new DateTime(2023, 1, 20);
            
            var fechaPrestamo3 = new DateTime(2023, 1, 10);
            var fechaVencimiento3 = new DateTime(2023, 1, 25);
            var fechaDevolucion3 = new DateTime(2023, 1, 23);
            
            var fechaPrestamo4 = new DateTime(2023, 1, 15);
            var fechaVencimiento4 = new DateTime(2023, 2, 5);

            // Seed Estudiantes
            modelBuilder.Entity<Estudiante>().HasData(
                new Estudiante { EstudianteId = 1, Nombre = "Ana García", Email = "ana.garcia@email.com", FechaInscripcion = new DateTime(2020, 9, 1), FechaCreacion = fechaPredeterminada },
                new Estudiante { EstudianteId = 2, Nombre = "Pedro Martínez", Email = "pedro.martinez@email.com", FechaInscripcion = new DateTime(2019, 9, 1), FechaCreacion = fechaPredeterminada },
                new Estudiante { EstudianteId = 3, Nombre = "María López", Email = "maria.lopez@email.com", FechaInscripcion = new DateTime(2021, 9, 1), FechaCreacion = fechaPredeterminada },
                new Estudiante { EstudianteId = 4, Nombre = "Carlos Rodríguez", Email = "carlos.rodriguez@email.com", FechaInscripcion = new DateTime(2018, 9, 1), FechaCreacion = fechaPredeterminada },
                new Estudiante { EstudianteId = 5, Nombre = "Laura Fernández", Email = "laura.fernandez@email.com", FechaInscripcion = new DateTime(2022, 9, 1), FechaCreacion = fechaPredeterminada }
            );

            // Seed Préstamos
            modelBuilder.Entity<Prestamo>().HasData(
                new Prestamo
                {
                    PrestamoId = 1,
                    LibroId = 1,
                    EstudianteId = 1,
                    FechaPrestamo = fechaPrestamo1,
                    FechaVencimiento = fechaVencimiento1,
                    FechaDevolucion = fechaDevolucion1,
                    Estado = EstadoPrestamo.Concluido,
                    FechaCreacion = fechaPredeterminada
                },
                new Prestamo
                {
                    PrestamoId = 2,
                    LibroId = 2,
                    EstudianteId = 2,
                    FechaPrestamo = fechaPrestamo2,
                    FechaVencimiento = fechaVencimiento2,
                    FechaDevolucion = null,
                    Estado = EstadoPrestamo.Expirado,
                    FechaCreacion = fechaPredeterminada
                },
                new Prestamo
                {
                    PrestamoId = 3,
                    LibroId = 3,
                    EstudianteId = 3,
                    FechaPrestamo = fechaPrestamo3,
                    FechaVencimiento = fechaVencimiento3,
                    FechaDevolucion = fechaDevolucion3,
                    Estado = EstadoPrestamo.Concluido,
                    FechaCreacion = fechaPredeterminada
                },
                new Prestamo
                {
                    PrestamoId = 4,
                    LibroId = 4,
                    EstudianteId = 4,
                    FechaPrestamo = fechaPrestamo4,
                    FechaVencimiento = fechaVencimiento4,
                    FechaDevolucion = null,
                    Estado = EstadoPrestamo.Activo,
                    FechaCreacion = fechaPredeterminada
                }
            );
        }
    }
}