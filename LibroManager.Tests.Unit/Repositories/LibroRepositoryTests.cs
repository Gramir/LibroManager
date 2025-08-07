using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibroManager.Tests.Repositories;

public class LibroRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly LibroRepository _repository;
    private static int _databaseNumber = 1;

    public LibroRepositoryTests()
    {
        // Usar un nombre único de base de datos para cada instancia de test
        var dbName = $"TestLibroManagerDb_Libros_{_databaseNumber++}";
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new LibroRepository(_context);

        // Limpiar la base de datos antes de cada test
        _context.Libros.RemoveRange(_context.Libros);
        _context.Prestamos.RemoveRange(_context.Prestamos);
        _context.Autores.RemoveRange(_context.Autores);
        _context.Categorias.RemoveRange(_context.Categorias);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllLibros()
    {
        // Arrange

        var libros = new List<Libro>
        {
            new() { Titulo = "Libro 1", ISBN = "1234567890" },
            new() { Titulo = "Libro 2", ISBN = "0987654321" }
        };
        await _context.Libros.AddRangeAsync(libros);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, l => l.Titulo == "Libro 1");
        Assert.Contains(result, l => l.Titulo == "Libro 2");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsLibro_WhenLibroExists()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(libro.LibroId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(libro.Titulo, result.Titulo);
        Assert.Equal(libro.ISBN, result.ISBN);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenLibroDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsNewLibro()
    {
        // Arrange
        var libro = new Libro { Titulo = "New Libro", ISBN = "1234567890" };

        // Act
        await _repository.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Libros.FindAsync(libro.LibroId);
        Assert.NotNull(result);
        Assert.Equal(libro.Titulo, result.Titulo);
        Assert.Equal(libro.ISBN, result.ISBN);
    }

    [Fact]
    public async Task Update_UpdatesExistingLibro()
    {
        // Arrange
        var libro = new Libro { Titulo = "Original Title", ISBN = "1234567890" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        libro.Titulo = "Updated Title";
        _repository.Update(libro);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Libros.FindAsync(libro.LibroId);
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Titulo);
    }

    [Fact]
    public async Task Remove_RemovesExistingLibro()
    {
        // Arrange
        var libro = new Libro { Titulo = "To Delete", ISBN = "1234567890" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(libro);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Libros.FindAsync(libro.LibroId);
        Assert.Null(result);
    }

    [Fact]
    public async Task IsbnExistsAsync_ReturnsTrue_WhenIsbnExists()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.IsbnExistsAsync("1234567890");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsbnExistsAsync_ReturnsFalse_WhenIsbnDoesNotExist()
    {
        // Act
        var result = await _repository.IsbnExistsAsync("1234567890");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SerialExistsAsync_ReturnsTrue_WhenSerialExists()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", Serial = "1234567890-1" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SerialExistsAsync("1234567890-1");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task SerialExistsAsync_ReturnsFalse_WhenSerialDoesNotExist()
    {
        // Act
        var result = await _repository.SerialExistsAsync("1234567890-1");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingLibros_WithTituloExpression()
    {
        // Arrange
        var libros = new List<Libro>
        {
            new() { Titulo = "Libro Programación" },
            new() { Titulo = "Libro Cocina" },
            new() { Titulo = "Libro Programación Avanzada" }
        };
        await _context.Libros.AddRangeAsync(libros);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(l => l.Titulo.Contains("Programación"));

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, l => Assert.Contains("Programación", l.Titulo));
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingLibros_WithIsbnExpression()
    {
        // Arrange
        var libros = new List<Libro>
        {
            new() { ISBN = "1234567890" },
            new() { ISBN = "0987654321" }
        };
        await _context.Libros.AddRangeAsync(libros);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(l => l.ISBN == "1234567890");

        // Assert
        Assert.Single(result);
        Assert.Equal("1234567890", result.First().ISBN);
    }

    [Fact]
    public async Task GetLibrosWithAutorAndCategoriaAsync_ReturnsLibrosWithoutDetails_WhenNoRelationships()
    {
        // Arrange
        var libros = new List<Libro>
        {
            new() { Titulo = "Libro 1", ISBN = "1234567890" },
            new() { Titulo = "Libro 2", ISBN = "0987654321" }
        };
        await _context.Libros.AddRangeAsync(libros);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLibrosWithAutorAndCategoriaAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, l => 
        {
            Assert.Null(l.Autor);
            Assert.Null(l.Categoria);
        });
    }
}