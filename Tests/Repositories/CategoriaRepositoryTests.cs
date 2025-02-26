using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibroManager.Tests.Repositories;

public class CategoriaRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly CategoriaRepository _repository;
    private static int _databaseNumber = 1;

    public CategoriaRepositoryTests()
    {
        var databaseName = $"TestLibroManagerDb_Categoria_{_databaseNumber++}";
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        _context = new ApplicationDbContext(_options);
        _repository = new CategoriaRepository(_context);
        
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        // Asegurarse de que no hay datos residuales en las tablas
        if (_context.Prestamos.Any())
        {
            _context.Prestamos.RemoveRange(_context.Prestamos);
        }
        
        if (_context.Libros.Any())
        {
            _context.Libros.RemoveRange(_context.Libros);
        }
        
        if (_context.Estudiantes.Any())
        {
            _context.Estudiantes.RemoveRange(_context.Estudiantes);
        }
        
        if (_context.Autores.Any())
        {
            _context.Autores.RemoveRange(_context.Autores);
        }
        
        if (_context.Categorias.Any())
        {
            _context.Categorias.RemoveRange(_context.Categorias);
        }
        
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByNombreAsync_ReturnsCategoria_WhenNombreExists()
    {
        // Arrange
        var categoria = new Categoria { Nombre = "Test Categoria" };
        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByNombreAsync("Test Categoria");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Categoria", result.Nombre);
    }

    [Fact]
    public async Task GetByNombreAsync_ReturnsNull_WhenNombreDoesNotExist()
    {
        // Act
        var result = await _repository.GetByNombreAsync("Nonexistent Category");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCategoriasWithLibrosAsync_ReturnsCategoriasWithLibros()
    {
        // Arrange
        var categoria1 = new Categoria { Nombre = "Categoria 1" };
        var categoria2 = new Categoria { Nombre = "Categoria 2" };
        await _context.Categorias.AddRangeAsync(new[] { categoria1, categoria2 });
        await _context.SaveChangesAsync();

        var libros = new List<Libro>
        {
            new() { 
                Titulo = "Libro 1", 
                ISBN = "1234567890", 
                CategoriaId = categoria1.CategoriaId 
            },
            new() { 
                Titulo = "Libro 2", 
                ISBN = "0987654321", 
                CategoriaId = categoria1.CategoriaId 
            }
        };
        await _context.Libros.AddRangeAsync(libros);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCategoriasWithLibrosAsync();

        // Assert
        Assert.Equal(2, result.Count());
        var categoriaConLibros = result.First(c => c.CategoriaId == categoria1.CategoriaId);
        Assert.Equal(2, categoriaConLibros.Libros?.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategorias()
    {
        // Arrange
        var categorias = new List<Categoria>
        {
            new() { Nombre = "Categoria 1" },
            new() { Nombre = "Categoria 2" }
        };
        await _context.Categorias.AddRangeAsync(categorias);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}