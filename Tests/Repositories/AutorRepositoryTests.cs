using LibroManager.Data.Context;
using Microsoft.EntityFrameworkCore.InMemory;
using LibroManager.Models;
using LibroManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibroManager.Tests.Repositories;

public class AutorRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly AutorRepository _repository;
    private static int _databaseNumber = 1;

    public AutorRepositoryTests()
    {
        var databaseName = $"TestLibroManagerDb_Autor_{_databaseNumber++}";
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new AutorRepository(_context);
        
        // Limpiar la base de datos antes de cada test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAutores()
    {
        // Arrange
        var autores = new List<Autor>
        {
            new() { Nombre = "Autor 1" },
            new() { Nombre = "Autor 2" }
        };
        await _context.Autores.AddRangeAsync(autores);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.Nombre == "Autor 1");
        Assert.Contains(result, a => a.Nombre == "Autor 2");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAutor_WhenAutorExists()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(autor.AutorId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(autor.Nombre, result.Nombre);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenAutorDoesNotExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsNewAutor()
    {
        // Arrange
        var autor = new Autor { Nombre = "New Autor" };

        // Act
        await _repository.AddAsync(autor);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Autores.FindAsync(autor.AutorId);
        Assert.NotNull(result);
        Assert.Equal(autor.Nombre, result.Nombre);
    }

    [Fact]
    public async Task Update_UpdatesExistingAutor()
    {
        // Arrange
        var autor = new Autor { Nombre = "Original Name" };
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();

        // Act
        autor.Nombre = "Updated Name";
        _repository.Update(autor);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Autores.FindAsync(autor.AutorId);
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Nombre);
    }

    [Fact]
    public async Task Remove_RemovesExistingAutor()
    {
        // Arrange
        var autor = new Autor { Nombre = "To Delete" };
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();

        // Act
        _repository.Remove(autor);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Autores.FindAsync(autor.AutorId);
        Assert.Null(result);
    }

    [Fact]
    public async Task HasLibrosAsync_ReturnsTrue_WhenAutorHasLibros()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();

        var libro = new Libro 
        { 
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = autor.AutorId
        };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasLibrosAsync(autor.AutorId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasLibrosAsync_ReturnsFalse_WhenAutorHasNoLibros()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.HasLibrosAsync(autor.AutorId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAutorWithLibrosAsync_ReturnsAutorWithLibros()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();

        var libros = new List<Libro>
        {
            new() { Titulo = "Libro 1", ISBN = "1234567890", AutorId = autor.AutorId },
            new() { Titulo = "Libro 2", ISBN = "0987654321", AutorId = autor.AutorId }
        };
        
        foreach (var libro in libros)
        {
            libro.Autor = autor;
        }
        
        await _context.Libros.AddRangeAsync(libros);
        await _context.SaveChangesAsync();

        // Limpiar el tracking para forzar una recarga fresca
        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetAutorWithLibrosAsync(autor.AutorId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Libros);
        Assert.Equal(2, result.Libros.Count);
        Assert.Contains(result.Libros!, l => l.Titulo == "Libro 1");
        Assert.Contains(result.Libros!, l => l.Titulo == "Libro 2");
    }
}