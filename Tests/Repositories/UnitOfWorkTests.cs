using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibroManager.Tests.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly DbContextOptions<ApplicationDbContext> _options;
    private readonly ApplicationDbContext _context;
    private readonly UnitOfWork _unitOfWork;
    private static int _databaseNumber = 1;

    public UnitOfWorkTests()
    {
        var databaseName = $"TestLibroManagerDb_UnitOfWork_{_databaseNumber++}";
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;

        _context = new ApplicationDbContext(_options);
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
        
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void Libros_ReturnsSameInstance()
    {
        // Act
        var repository1 = _unitOfWork.Libros;
        var repository2 = _unitOfWork.Libros;

        // Assert
        Assert.Same(repository1, repository2);
    }

    [Fact]
    public void Autores_ReturnsSameInstance()
    {
        // Act
        var repository1 = _unitOfWork.Autores;
        var repository2 = _unitOfWork.Autores;

        // Assert
        Assert.Same(repository1, repository2);
    }

    [Fact]
    public async Task SaveChangesAsync_SavesChangesToAllRepositories()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        var libro = new Libro { Titulo = "Test Libro", ISBN = "1234567890" };

        await _unitOfWork.Autores.AddAsync(autor);
        await _unitOfWork.SaveChangesAsync();

        libro.AutorId = autor.AutorId;
        libro.Autor = autor;
        await _unitOfWork.Libros.AddAsync(libro);

        // Act
        var savedEntries = await _unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(1, savedEntries);
        var savedAutor = await _context.Autores.FirstOrDefaultAsync();
        var savedLibro = await _context.Libros.FirstOrDefaultAsync();
        
        Assert.NotNull(savedAutor);
        Assert.NotNull(savedLibro);
        Assert.Equal("Test Autor", savedAutor.Nombre);
        Assert.Equal("Test Libro", savedLibro.Titulo);
    }

    [Fact]
    public async Task UnitOfWork_MantieneLaConsistenciaDeLosRepositorios()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        await _unitOfWork.Autores.AddAsync(autor);
        await _unitOfWork.SaveChangesAsync();

        var libro = new Libro 
        { 
            Titulo = "Test Libro", 
            ISBN = "1234567890",
            AutorId = autor.AutorId
        };
        
        await _unitOfWork.Libros.AddAsync(libro);
        await _unitOfWork.SaveChangesAsync();

        // Act
        var autorConLibros = await _context.Autores
            .Include(a => a.Libros)
            .FirstOrDefaultAsync(a => a.AutorId == autor.AutorId);

        var libroConAutor = await _context.Libros
            .Include(l => l.Autor)
            .FirstOrDefaultAsync(l => l.LibroId == libro.LibroId);

        // Assert
        Assert.NotNull(autorConLibros);
        Assert.NotNull(autorConLibros.Libros);
        Assert.Single(autorConLibros.Libros);
        Assert.Equal(libro.Titulo, autorConLibros.Libros.First().Titulo);
        
        Assert.NotNull(libroConAutor);
        Assert.NotNull(libroConAutor.Autor);
        Assert.Equal(autor.Nombre, libroConAutor.Autor.Nombre);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}