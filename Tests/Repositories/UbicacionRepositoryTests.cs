using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibroManager.Tests.Repositories;

public class UbicacionRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly UbicacionRepository _repository;
    private static int _databaseNumber = 1;

    public UbicacionRepositoryTests()
    {
        // Usar un nombre único de base de datos para cada instancia de test
        var dbName = $"TestLibroManagerDb_Ubicaciones_{_databaseNumber++}";
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new UbicacionRepository(_context);
        
        // Limpiar la base de datos antes de cada test
        _context.Ubicaciones.RemoveRange(_context.Ubicaciones);
        _context.Libros.RemoveRange(_context.Libros);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUbicaciones()
    {
        // Arrange
        var ubicaciones = new List<Ubicacion>
        {
            new() { Estante = "A", Nivel = 1, Posicion = 1 },
            new() { Estante = "B", Nivel = 2, Posicion = 1 }
        };
        await _context.Ubicaciones.AddRangeAsync(ubicaciones);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.Estante == "A" && u.Nivel == 1);
        Assert.Contains(result, u => u.Estante == "B" && u.Nivel == 2);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUbicacion_WhenUbicacionExists()
    {
        // Arrange
        var ubicacion = new Ubicacion { Estante = "C", Nivel = 1, Posicion = 1 };
        await _context.Ubicaciones.AddAsync(ubicacion);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(ubicacion.UbicacionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ubicacion.Estante, result.Estante);
        Assert.Equal(ubicacion.Nivel, result.Nivel);
        Assert.Equal(ubicacion.Posicion, result.Posicion);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenUbicacionDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task FindAsync_ReturnsFilteredUbicaciones()
    {
        // Arrange
        var ubicaciones = new List<Ubicacion>
        {
            new() { Estante = "A", Nivel = 1, Posicion = 1 },
            new() { Estante = "B", Nivel = 2, Posicion = 1 },
            new() { Estante = "C", Nivel = 1, Posicion = 2 }
        };
        await _context.Ubicaciones.AddRangeAsync(ubicaciones);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(u => u.Nivel == 1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.Equal(1, u.Nivel));
    }

    [Fact]
    public async Task AddAsync_AddsNewUbicacion()
    {
        // Arrange
        var ubicacion = new Ubicacion { Estante = "D", Nivel = 3, Posicion = 1 };

        // Act
        await _repository.AddAsync(ubicacion);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Ubicaciones.FindAsync(ubicacion.UbicacionId);
        Assert.NotNull(result);
        Assert.Equal(ubicacion.Estante, result.Estante);
        Assert.Equal(ubicacion.Nivel, result.Nivel);
        Assert.Equal(ubicacion.Posicion, result.Posicion);
    }

    [Fact]
    public async Task Update_UpdatesExistingUbicacion()
    {
        // Arrange
        var ubicacion = new Ubicacion { Estante = "E", Nivel = 1, Posicion = 1 };
        await _context.Ubicaciones.AddAsync(ubicacion);
        await _context.SaveChangesAsync();

        // Act
        ubicacion.Estante = "F";
        _repository.Update(ubicacion);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Ubicaciones.FindAsync(ubicacion.UbicacionId);
        Assert.NotNull(result);
        Assert.Equal("F", result.Estante);
    }

    [Fact]
    public async Task Remove_RemovesExistingUbicacion()
    {
        // Arrange
        var ubicacion = new Ubicacion { Estante = "G", Nivel = 1, Posicion = 1 };
        await _context.Ubicaciones.AddAsync(ubicacion);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(ubicacion);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Ubicaciones.FindAsync(ubicacion.UbicacionId);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_IncludesLibros_WhenUbicacionHasLibros()
    {
        // Arrange
        var ubicacion = new Ubicacion { Estante = "H", Nivel = 2, Posicion = 1 };
        await _context.Ubicaciones.AddAsync(ubicacion);
        await _context.SaveChangesAsync();

        var libros = new List<Libro>
        {
            new() { Titulo = "Libro 1", ISBN = "1234567890", UbicacionId = ubicacion.UbicacionId },
            new() { Titulo = "Libro 2", ISBN = "0987654321", UbicacionId = ubicacion.UbicacionId }
        };
        await _context.Libros.AddRangeAsync(libros);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var ubicacionResult = result.FirstOrDefault(u => u.UbicacionId == ubicacion.UbicacionId);

        // Assert
        Assert.NotNull(ubicacionResult);
        Assert.NotNull(ubicacionResult.Libros);
        Assert.Equal(2, ubicacionResult.Libros.Count);
    }

    [Fact]
    public async Task HasLibrosAsync_ReturnsTrue_WhenUbicacionHasLibros()
    {
        // Arrange
        var ubicacion = new Ubicacion { Estante = "I", Nivel = 1, Posicion = 1 };
        await _context.Ubicaciones.AddAsync(ubicacion);
        await _context.SaveChangesAsync();

        var libro = new Libro { Titulo = "Libro 3", ISBN = "5678901234", UbicacionId = ubicacion.UbicacionId };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasLibrosAsync(ubicacion.UbicacionId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasLibrosAsync_ReturnsFalse_WhenUbicacionHasNoLibros()
    {
        // Arrange
        var ubicacion = new Ubicacion { Estante = "J", Nivel = 3, Posicion = 1 };
        await _context.Ubicaciones.AddAsync(ubicacion);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasLibrosAsync(ubicacion.UbicacionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetUbicacionWithLibrosAsync_ReturnsUbicacionWithLibros()
    {
        // Arrange
        var ubicacion = new Ubicacion { Estante = "K", Nivel = 2, Posicion = 1 };
        await _context.Ubicaciones.AddAsync(ubicacion);
        await _context.SaveChangesAsync();

        var libros = new List<Libro>
        {
            new() { Titulo = "Libro 4", ISBN = "1111222233", UbicacionId = ubicacion.UbicacionId },
            new() { Titulo = "Libro 5", ISBN = "4444555566", UbicacionId = ubicacion.UbicacionId }
        };
        await _context.Libros.AddRangeAsync(libros);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUbicacionWithLibrosAsync(ubicacion.UbicacionId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Libros);
        Assert.Equal(2, result.Libros.Count);
        Assert.Contains(result.Libros, l => l.Titulo == "Libro 4");
        Assert.Contains(result.Libros, l => l.Titulo == "Libro 5");
    }
    
    [Fact]
    public async Task GetUbicacionWithLibrosAsync_ReturnsNull_WhenUbicacionDoesNotExist()
    {
        // Act
        var result = await _repository.GetUbicacionWithLibrosAsync(999);

        // Assert
        Assert.Null(result);
    }
}