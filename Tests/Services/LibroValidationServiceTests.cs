using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LibroManager.Tests.Services;

public class LibroValidationServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly LibroValidationService _validationService;

    public LibroValidationServiceTests()
    {
        // Configurar la base de datos en memoria para los tests
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _validationService = new LibroValidationService(_context);
    }

    [Fact]
    public async Task LibroEsValido_ConAutorYCategoriaValidos_ReturnTrue()
    {
        // Arrange
        _context.Autores.Add(new Autor { AutorId = 1, Nombre = "Test Autor" });
        _context.Categorias.Add(new Categoria { CategoriaId = 1, Nombre = "Test Categoria" });
        await _context.SaveChangesAsync();

        var libro = new Libro
        {
            AutorId = 1,
            CategoriaId = 1
        };

        // Act
        var result = await _validationService.LibroEsValido(libro);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public async Task LibroEsValido_ConIdsInvalidos_ReturnFalse(int autorId, int categoriaId)
    {
        // Arrange
        var libro = new Libro
        {
            AutorId = autorId,
            CategoriaId = categoriaId
        };

        // Act
        var result = await _validationService.LibroEsValido(libro);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task PuedeEliminarAutor_SinLibros_ReturnTrue()
    {
        // Arrange
        _context.Autores.Add(new Autor { AutorId = 1, Nombre = "Test Autor" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _validationService.PuedeEliminarAutor(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PuedeEliminarAutor_ConLibros_ReturnFalse()
    {
        // Arrange
        _context.Autores.Add(new Autor { AutorId = 1, Nombre = "Test Autor" });
        _context.Libros.Add(new Libro { LibroId = 1, Titulo = "Test Libro", AutorId = 1, CategoriaId = 1 });
        await _context.SaveChangesAsync();

        // Act
        var result = await _validationService.PuedeEliminarAutor(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task LibroEstaPrestado_ConPrestamoActivo_ReturnTrue()
    {
        // Arrange
        _context.Libros.Add(new Libro { LibroId = 1, Titulo = "Test Libro", AutorId = 1, CategoriaId = 1 });
        _context.Prestamos.Add(new Prestamo
        {
            PrestamoId = 1,
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7),
            Estado = EstadoPrestamo.Activo
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _validationService.LibroEstaPrestado(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task LibroEstaPrestado_SinPrestamoActivo_ReturnFalse()
    {
        // Arrange
        _context.Libros.Add(new Libro { LibroId = 2, Titulo = "Test Libro 2", AutorId = 1, CategoriaId = 1 });
        _context.Prestamos.Add(new Prestamo
        {
            PrestamoId = 2,
            LibroId = 2,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now.AddDays(-10),
            FechaVencimiento = DateTime.Now.AddDays(-3),
            FechaDevolucion = DateTime.Now.AddDays(-5),
            Estado = EstadoPrestamo.Concluido
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _validationService.LibroEstaPrestado(2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FechasPrestamoSonValidas_FechaPrestamoAnteriorAVencimiento_ReturnTrue()
    {
        // Arrange
        var fechaPrestamo = DateTime.Now;
        var fechaVencimiento = fechaPrestamo.AddDays(7);

        // Act
        var result = _validationService.FechasPrestamoSonValidas(fechaPrestamo, fechaVencimiento);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FechasPrestamoSonValidas_FechaPrestamoIgualAVencimiento_ReturnFalse()
    {
        // Arrange
        var fecha = DateTime.Now;

        // Act
        var result = _validationService.FechasPrestamoSonValidas(fecha, fecha);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FechasPrestamoSonValidas_FechaVencimientoAnteriorAPrestamo_ReturnFalse()
    {
        // Arrange
        var fechaPrestamo = DateTime.Now;
        var fechaVencimiento = fechaPrestamo.AddDays(-1);

        // Act
        var result = _validationService.FechasPrestamoSonValidas(fechaPrestamo, fechaVencimiento);

        // Assert
        Assert.False(result);
    }
}