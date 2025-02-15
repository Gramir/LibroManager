using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibroManager.Tests.Repositories;

public class PrestamoRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly PrestamoRepository _repository;
    private static int _databaseNumber = 1;

    public PrestamoRepositoryTests()
    {
        var databaseName = $"TestLibroManagerDb_Prestamo_{_databaseNumber++}";
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new PrestamoRepository(_context);
        
        // Limpiar la base de datos antes de cada test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPrestamos()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        var estudiante = new Estudiante { Nombre = "Test Estudiante", Email = "test@test.com" };
        await _context.Libros.AddAsync(libro);
        await _context.Estudiantes.AddAsync(estudiante);
        await _context.SaveChangesAsync();

        var prestamos = new List<Prestamo>
        {
            new() { LibroId = libro.LibroId, EstudianteId = estudiante.EstudianteId, FechaPrestamo = DateTime.Now, FechaVencimiento = DateTime.Now.AddDays(7) },
            new() { LibroId = libro.LibroId, EstudianteId = estudiante.EstudianteId, FechaPrestamo = DateTime.Now, FechaVencimiento = DateTime.Now.AddDays(14) }
        };
        await _context.Prestamos.AddRangeAsync(prestamos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetPrestamosByEstudianteAsync_ReturnsPrestamosForEstudiante()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        var estudiante1 = new Estudiante { Nombre = "Test Estudiante 1", Email = "test1@test.com" };
        var estudiante2 = new Estudiante { Nombre = "Test Estudiante 2", Email = "test2@test.com" };
        await _context.Libros.AddAsync(libro);
        await _context.Estudiantes.AddRangeAsync(new[] { estudiante1, estudiante2 });
        await _context.SaveChangesAsync();

        var prestamos = new List<Prestamo>
        {
            new() { LibroId = libro.LibroId, EstudianteId = estudiante1.EstudianteId, FechaPrestamo = DateTime.Now, FechaVencimiento = DateTime.Now.AddDays(7) },
            new() { LibroId = libro.LibroId, EstudianteId = estudiante2.EstudianteId, FechaPrestamo = DateTime.Now, FechaVencimiento = DateTime.Now.AddDays(7) }
        };
        await _context.Prestamos.AddRangeAsync(prestamos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPrestamosByEstudianteAsync(estudiante1.EstudianteId);

        // Assert
        Assert.Single(result);
        Assert.Equal(estudiante1.EstudianteId, result.First().EstudianteId);
    }

    [Fact]
    public async Task GetPrestamosByLibroAsync_ReturnsPrestamosForLibro()
    {
        // Arrange
        var libro1 = new Libro { Titulo = "Test Libro 1", ISBN = "1234567890" };
        var libro2 = new Libro { Titulo = "Test Libro 2", ISBN = "0987654321" };
        var estudiante = new Estudiante { Nombre = "Test Estudiante", Email = "test@test.com" };
        await _context.Libros.AddRangeAsync(new[] { libro1, libro2 });
        await _context.Estudiantes.AddAsync(estudiante);
        await _context.SaveChangesAsync();

        var prestamos = new List<Prestamo>
        {
            new() { LibroId = libro1.LibroId, EstudianteId = estudiante.EstudianteId, FechaPrestamo = DateTime.Now, FechaVencimiento = DateTime.Now.AddDays(7) },
            new() { LibroId = libro2.LibroId, EstudianteId = estudiante.EstudianteId, FechaPrestamo = DateTime.Now, FechaVencimiento = DateTime.Now.AddDays(7) }
        };
        await _context.Prestamos.AddRangeAsync(prestamos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPrestamosByLibroAsync(libro1.LibroId);

        // Assert
        Assert.Single(result);
        Assert.Equal(libro1.LibroId, result.First().LibroId);
    }

    [Fact]
    public async Task GetPrestamosActivosAsync_ReturnsActivePrestamos()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        var estudiante = new Estudiante { Nombre = "Test Estudiante", Email = "test@test.com" };
        await _context.Libros.AddAsync(libro);
        await _context.Estudiantes.AddAsync(estudiante);
        await _context.SaveChangesAsync();

        var prestamos = new List<Prestamo>
        {
            new() { LibroId = libro.LibroId, EstudianteId = estudiante.EstudianteId, FechaPrestamo = DateTime.Now, FechaVencimiento = DateTime.Now.AddDays(7) },
            new() { LibroId = libro.LibroId, EstudianteId = estudiante.EstudianteId, FechaPrestamo = DateTime.Now.AddDays(-14), FechaVencimiento = DateTime.Now.AddDays(-7) }
        };
        await _context.Prestamos.AddRangeAsync(prestamos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPrestamosActivosAsync();

        // Assert
        Assert.Single(result);
        Assert.True(result.First().FechaVencimiento >= DateTime.Now);
    }
}