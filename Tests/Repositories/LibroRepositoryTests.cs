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
    public async Task GetLibrosWithAutorAndCategoriaAsync_ReturnsLibrosWithDetails()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        var categoria = new Categoria { Nombre = "Test Categoria" };
        await _context.Autores.AddAsync(autor);
        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        var libro = new Libro
        {
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = autor.AutorId,
            CategoriaId = categoria.CategoriaId
        };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLibrosWithAutorAndCategoriaAsync();

        // Assert
        var libroResult = result.First();
        Assert.NotNull(libroResult.Autor);
        Assert.NotNull(libroResult.Categoria);
        Assert.Equal(autor.Nombre, libroResult.Autor.Nombre);
        Assert.Equal(categoria.Nombre, libroResult.Categoria.Nombre);
    }

    [Fact]
    public async Task GetLibroWithDetailsAsync_ReturnsLibroWithAllDetails()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        var categoria = new Categoria { Nombre = "Test Categoria" };
        await _context.Autores.AddAsync(autor);
        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        var libro = new Libro
        {
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = autor.AutorId,
            CategoriaId = categoria.CategoriaId
        };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        var prestamo = new Prestamo
        {
            LibroId = libro.LibroId,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };
        await _context.Prestamos.AddAsync(prestamo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetLibroWithDetailsAsync(libro.LibroId);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Autor);
        Assert.NotNull(result.Categoria);
        Assert.NotNull(result.Prestamos);
        Assert.Single(result.Prestamos);
        Assert.Equal(autor.Nombre, result.Autor.Nombre);
        Assert.Equal(categoria.Nombre, result.Categoria.Nombre);
    }

    [Fact]
    public async Task EstaPrestadoAsync_ReturnsTrue_WhenLibroTienePrestamoActivo()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        var prestamo = new Prestamo
        {
            LibroId = libro.LibroId,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };
        await _context.Prestamos.AddAsync(prestamo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EstaPrestadoAsync(libro.LibroId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EstaPrestadoAsync_ReturnsFalse_WhenLibroNoTienePrestamoActivo()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EstaPrestadoAsync(libro.LibroId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EstaPrestadoAsync_ReturnsFalse_WhenPrestamoEstaVencido()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        var prestamo = new Prestamo
        {
            LibroId = libro.LibroId,
            FechaPrestamo = DateTime.Now.AddDays(-14),
            FechaVencimiento = DateTime.Now.AddDays(-7)
        };
        await _context.Prestamos.AddAsync(prestamo);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EstaPrestadoAsync(libro.LibroId);

        // Assert
        Assert.False(result);
    }
}