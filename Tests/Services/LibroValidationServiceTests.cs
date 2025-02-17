using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace LibroManager.Tests.Services;

public class LibroValidationServiceTests
{
    private readonly Mock<DbSet<Autor>> _mockAutores;
    private readonly Mock<DbSet<Categoria>> _mockCategorias;
    private readonly Mock<DbSet<Libro>> _mockLibros;
    private readonly Mock<DbSet<Prestamo>> _mockPrestamos;
    private readonly Mock<DbSet<Estudiante>> _mockEstudiantes;
    private readonly LibroValidationService _validationService;
    private readonly ApplicationDbContext _context;

    public LibroValidationServiceTests()
    {
        _mockAutores = MockDbSet<Autor>();
        _mockCategorias = MockDbSet<Categoria>();
        _mockLibros = MockDbSet<Libro>();
        _mockPrestamos = MockDbSet<Prestamo>();
        _mockEstudiantes = MockDbSet<Estudiante>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestLibroManagerDb")
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _validationService = new LibroValidationService(_context);
    }

    private Mock<DbSet<T>> MockDbSet<T>() where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new List<T>().AsQueryable().Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(new List<T>().AsQueryable().Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(new List<T>().AsQueryable().ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(new List<T>().GetEnumerator());
        return mockSet;
    }

    [Fact]
    public async Task LibroEsValido_ConAutorYCategoriaValidos_ReturnTrue()
    {
        // Arrange
        var autor = new Autor { AutorId = 1, Nombre = "Test Autor" };
        var categoria = new Categoria { CategoriaId = 1, Nombre = "Test Categoria" };
        await _context.Autores.AddAsync(autor);
        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        var libro = new Libro { AutorId = autor.AutorId, CategoriaId = categoria.CategoriaId };

        // Act
        var result = await _validationService.LibroEsValido(libro);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task LibroEsValido_ConAutorInvalido_ReturnFalse()
    {
        // Arrange
        var categoria = new Categoria { CategoriaId = 1, Nombre = "Test Categoria" };
        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        var libro = new Libro { AutorId = 999, CategoriaId = categoria.CategoriaId };

        // Act
        var result = await _validationService.LibroEsValido(libro);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public async Task LibroEsValido_ConIdsInvalidos_ReturnFalse(int autorId, int categoriaId)
    {
        // Arrange
        var libro = new Libro { AutorId = autorId, CategoriaId = categoriaId };

        // Act
        var result = await _validationService.LibroEsValido(libro);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PuedeEliminarAutor_SinLibros_ReturnTrue()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();

        // Act
        var result = await _validationService.PuedeEliminarAutor(autor.AutorId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PuedeEliminarAutor_ConLibros_ReturnFalse()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };
        await _context.Autores.AddAsync(autor);
        await _context.SaveChangesAsync();

        var libro = new Libro { Titulo = "Test Libro", AutorId = autor.AutorId };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _validationService.PuedeEliminarAutor(autor.AutorId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task LibroEstaPrestado_ConPrestamoActivo_ReturnTrue()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro" };
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
        var result = await _validationService.LibroEstaPrestado(libro.LibroId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task LibroEstaPrestado_SinPrestamoActivo_ReturnFalse()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro" };
        await _context.Libros.AddAsync(libro);
        await _context.SaveChangesAsync();

        // Act
        var result = await _validationService.LibroEstaPrestado(libro.LibroId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PrestamoEsValido_ConDatosValidos_ReturnTrue()
    {
        // Arrange
        var libro = new Libro { Titulo = "Test Libro" };
        var estudiante = new Estudiante { Nombre = "Test Estudiante" };
        await _context.Libros.AddAsync(libro);
        await _context.Estudiantes.AddAsync(estudiante);
        await _context.SaveChangesAsync();

        var prestamo = new Prestamo { 
            LibroId = libro.LibroId, 
            EstudianteId = estudiante.EstudianteId,
            FechaPrestamo = DateTime.Today,
            FechaVencimiento = DateTime.Today.AddDays(7)
        };

        // Act
        var result = await _validationService.PrestamoEsValido(prestamo);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    public async Task PrestamoEsValido_ConIdsInvalidos_ReturnFalse(int libroId, int estudianteId)
    {
        // Arrange
        var prestamo = new Prestamo { LibroId = libroId, EstudianteId = estudianteId };

        // Act
        var result = await _validationService.PrestamoEsValido(prestamo);

        // Assert
        Assert.False(result);
    }
}