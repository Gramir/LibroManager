using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibroManager.Tests.Repositories;

public class EstudianteRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly EstudianteRepository _repository;
    private static int _databaseNumber = 1;

    public EstudianteRepositoryTests()
    {
        var databaseName = $"TestLibroManagerDb_Estudiante_{_databaseNumber++}";
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new EstudianteRepository(_context);
        
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        // Limpiar cualquier dato existente para asegurarnos de que el test comienza con un estado limpio
        if (_context.Estudiantes.Any())
        {
            _context.Estudiantes.RemoveRange(_context.Estudiantes);
        }
        
        if (_context.Libros.Any())
        {
            _context.Libros.RemoveRange(_context.Libros);
        }
        
        if (_context.Prestamos.Any())
        {
            _context.Prestamos.RemoveRange(_context.Prestamos);
        }
        
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsEstudiante_WhenEmailExists()
    {
        // Arrange
        var estudiante = new Estudiante { 
            Nombre = "Test Estudiante", 
            Email = "test@test.com",
            FechaInscripcion = DateTime.Now 
        };
        await _context.Estudiantes.AddAsync(estudiante);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("test@test.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenEmailDoesNotExist()
    {
        // Act
        var result = await _repository.GetByEmailAsync("nonexistent@test.com");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetEstudiantesWithPrestamosActivosAsync_ReturnsEstudiantesWithActivePrestamos()
    {
        // Arrange
        var estudiante1 = new Estudiante { 
            Nombre = "Estudiante 1", 
            Email = "estudiante1@test.com",
            FechaInscripcion = DateTime.Now 
        };
        var estudiante2 = new Estudiante { 
            Nombre = "Estudiante 2", 
            Email = "estudiante2@test.com",
            FechaInscripcion = DateTime.Now 
        };
        await _context.Estudiantes.AddRangeAsync(new[] { estudiante1, estudiante2 });
        
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        var prestamos = new List<Prestamo>
        {
            new() { 
                LibroId = libro.LibroId, 
                EstudianteId = estudiante1.EstudianteId, 
                FechaPrestamo = DateTime.Now, 
                FechaVencimiento = DateTime.Now.AddDays(7) 
            },
            new() { 
                LibroId = libro.LibroId, 
                EstudianteId = estudiante2.EstudianteId, 
                FechaPrestamo = DateTime.Now.AddDays(-14), 
                FechaVencimiento = DateTime.Now.AddDays(-7) 
            }
        };
        await _context.Prestamos.AddRangeAsync(prestamos);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetEstudiantesWithPrestamosActivosAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(estudiante1.EstudianteId, result.First().EstudianteId);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEstudiantes()
    {
        // Arrange
        var estudiantes = new List<Estudiante>
        {
            new() { 
                Nombre = "Estudiante 1", 
                Email = "estudiante1@test.com",
                FechaInscripcion = DateTime.Now 
            },
            new() { 
                Nombre = "Estudiante 2", 
                Email = "estudiante2@test.com",
                FechaInscripcion = DateTime.Now 
            }
        };
        await _context.Estudiantes.AddRangeAsync(estudiantes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}