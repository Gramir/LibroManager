using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class LibroValidationServiceTests
{
    private readonly Mock<DbSet<Autor>> _autorDbSetMock;
    private readonly Mock<DbSet<Categoria>> _categoriaDbSetMock;
    private readonly Mock<DbSet<Libro>> _libroDbSetMock;
    private readonly Mock<DbSet<Prestamo>> _prestamoDbSetMock;
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly LibroValidationService _validationService;

    public LibroValidationServiceTests()
    {
        _autorDbSetMock = MockDbSet<Autor>();
        _categoriaDbSetMock = MockDbSet<Categoria>();
        _libroDbSetMock = MockDbSet<Libro>();
        _prestamoDbSetMock = MockDbSet<Prestamo>();

        _contextMock = new Mock<ApplicationDbContext>();
        _contextMock.Setup(c => c.Autores).Returns(_autorDbSetMock.Object);
        _contextMock.Setup(c => c.Categorias).Returns(_categoriaDbSetMock.Object);
        _contextMock.Setup(c => c.Libros).Returns(_libroDbSetMock.Object);
        _contextMock.Setup(c => c.Prestamos).Returns(_prestamoDbSetMock.Object);

        _validationService = new LibroValidationService(_contextMock.Object);
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
        var libro = new Libro 
        { 
            AutorId = 1, 
            CategoriaId = 1 
        };

        var autores = new List<Autor> 
        { 
            new() { AutorId = 1 } 
        }.AsQueryable();

        var categorias = new List<Categoria> 
        { 
            new() { CategoriaId = 1 } 
        }.AsQueryable();

        _autorDbSetMock.As<IQueryable<Autor>>().Setup(m => m.Provider).Returns(autores.Provider);
        _autorDbSetMock.As<IQueryable<Autor>>().Setup(m => m.Expression).Returns(autores.Expression);
        _autorDbSetMock.As<IQueryable<Autor>>().Setup(m => m.ElementType).Returns(autores.ElementType);
        _autorDbSetMock.As<IQueryable<Autor>>().Setup(m => m.GetEnumerator()).Returns(autores.GetEnumerator);

        _categoriaDbSetMock.As<IQueryable<Categoria>>().Setup(m => m.Provider).Returns(categorias.Provider);
        _categoriaDbSetMock.As<IQueryable<Categoria>>().Setup(m => m.Expression).Returns(categorias.Expression);
        _categoriaDbSetMock.As<IQueryable<Categoria>>().Setup(m => m.ElementType).Returns(categorias.ElementType);
        _categoriaDbSetMock.As<IQueryable<Categoria>>().Setup(m => m.GetEnumerator()).Returns(categorias.GetEnumerator);

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
        var libros = new List<Libro>().AsQueryable();

        _libroDbSetMock.As<IQueryable<Libro>>().Setup(m => m.Provider).Returns(libros.Provider);
        _libroDbSetMock.As<IQueryable<Libro>>().Setup(m => m.Expression).Returns(libros.Expression);
        _libroDbSetMock.As<IQueryable<Libro>>().Setup(m => m.ElementType).Returns(libros.ElementType);
        _libroDbSetMock.As<IQueryable<Libro>>().Setup(m => m.GetEnumerator()).Returns(libros.GetEnumerator);

        // Act
        var result = await _validationService.PuedeEliminarAutor(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task PuedeEliminarAutor_ConLibros_ReturnFalse()
    {
        // Arrange
        var libros = new List<Libro> 
        { 
            new() { AutorId = 1 } 
        }.AsQueryable();

        _libroDbSetMock.As<IQueryable<Libro>>().Setup(m => m.Provider).Returns(libros.Provider);
        _libroDbSetMock.As<IQueryable<Libro>>().Setup(m => m.Expression).Returns(libros.Expression);
        _libroDbSetMock.As<IQueryable<Libro>>().Setup(m => m.ElementType).Returns(libros.ElementType);
        _libroDbSetMock.As<IQueryable<Libro>>().Setup(m => m.GetEnumerator()).Returns(libros.GetEnumerator);

        // Act
        var result = await _validationService.PuedeEliminarAutor(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task LibroEstaPrestado_ConPrestamoActivo_ReturnTrue()
    {
        // Arrange
        var prestamos = new List<Prestamo> 
        { 
            new() 
            { 
                LibroId = 1, 
                Estado = EstadoPrestamo.Activo 
            } 
        }.AsQueryable();

        _prestamoDbSetMock.As<IQueryable<Prestamo>>().Setup(m => m.Provider).Returns(prestamos.Provider);
        _prestamoDbSetMock.As<IQueryable<Prestamo>>().Setup(m => m.Expression).Returns(prestamos.Expression);
        _prestamoDbSetMock.As<IQueryable<Prestamo>>().Setup(m => m.ElementType).Returns(prestamos.ElementType);
        _prestamoDbSetMock.As<IQueryable<Prestamo>>().Setup(m => m.GetEnumerator()).Returns(prestamos.GetEnumerator);

        // Act
        var result = await _validationService.LibroEstaPrestado(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task LibroEstaPrestado_SinPrestamoActivo_ReturnFalse()
    {
        // Arrange
        var prestamos = new List<Prestamo> 
        { 
            new() 
            { 
                LibroId = 1, 
                Estado = EstadoPrestamo.Concluido 
            } 
        }.AsQueryable();

        _prestamoDbSetMock.As<IQueryable<Prestamo>>().Setup(m => m.Provider).Returns(prestamos.Provider);
        _prestamoDbSetMock.As<IQueryable<Prestamo>>().Setup(m => m.Expression).Returns(prestamos.Expression);
        _prestamoDbSetMock.As<IQueryable<Prestamo>>().Setup(m => m.ElementType).Returns(prestamos.ElementType);
        _prestamoDbSetMock.As<IQueryable<Prestamo>>().Setup(m => m.GetEnumerator()).Returns(prestamos.GetEnumerator);

        // Act
        var result = await _validationService.LibroEstaPrestado(1);

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