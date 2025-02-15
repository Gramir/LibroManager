using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using LibroManager.Services.Interfaces;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class LibroServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILibroValidationService> _mockValidationService;
    private readonly Mock<ILibroRepository> _mockLibroRepository;
    private readonly LibroService _libroService;

    public LibroServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockValidationService = new Mock<ILibroValidationService>();
        _mockLibroRepository = new Mock<ILibroRepository>();

        _mockUnitOfWork.Setup(u => u.Libros).Returns(_mockLibroRepository.Object);
        
        _libroService = new LibroService(_mockUnitOfWork.Object, _mockValidationService.Object);
    }

    [Fact]
    public async Task CreateLibroAsync_WithValidLibro_ReturnsTrue()
    {
        // Arrange
        var libro = new Libro 
        { 
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1
        };

        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(true);
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _libroService.CreateLibroAsync(libro);

        // Assert
        Assert.True(result);
        _mockLibroRepository.Verify(r => r.AddAsync(It.IsAny<Libro>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateLibroAsync_WithDuplicateIsbn_ReturnsFalse()
    {
        // Arrange
        var libro = new Libro 
        { 
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1
        };

        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(true);
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _libroService.CreateLibroAsync(libro);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.AddAsync(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteLibroAsync_WithPrestadoLibro_ReturnsFalse()
    {
        // Arrange
        int libroId = 1;
        var libro = new Libro { LibroId = libroId };

        _mockLibroRepository.Setup(r => r.GetByIdAsync(libroId))
            .ReturnsAsync(libro);
        _mockLibroRepository.Setup(r => r.EstaPrestadoAsync(libroId))
            .ReturnsAsync(true);

        // Act
        var result = await _libroService.DeleteLibroAsync(libroId);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.Remove(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllLibrosAsync_ReturnsAllLibros()
    {
        // Arrange
        var libros = new List<Libro>
        {
            new() { LibroId = 1, Titulo = "Libro 1" },
            new() { LibroId = 2, Titulo = "Libro 2" }
        };
        _mockLibroRepository.Setup(r => r.GetLibrosWithAutorAndCategoriaAsync())
            .ReturnsAsync(libros);

        // Act
        var result = await _libroService.GetAllLibrosAsync();

        // Assert
        Assert.Equal(libros, result);
        _mockLibroRepository.Verify(r => r.GetLibrosWithAutorAndCategoriaAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLibroByIdAsync_ReturnsLibro()
    {
        // Arrange
        var libro = new Libro { LibroId = 1, Titulo = "Test Libro" };
        _mockLibroRepository.Setup(r => r.GetLibroWithDetailsAsync(1))
            .ReturnsAsync(libro);

        // Act
        var result = await _libroService.GetLibroByIdAsync(1);

        // Assert
        Assert.Equal(libro, result);
        _mockLibroRepository.Verify(r => r.GetLibroWithDetailsAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreateLibroAsync_WithInvalidLibro_ReturnsFalse()
    {
        // Arrange
        var libro = new Libro 
        { 
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1
        };

        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(false);

        // Act
        var result = await _libroService.CreateLibroAsync(libro);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.AddAsync(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateLibroAsync_WithValidLibroAndNewIsbn_ReturnsTrue()
    {
        // Arrange
        var libro = new Libro 
        { 
            LibroId = 1,
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1
        };

        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(true);
        _mockLibroRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(libro);
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _libroService.UpdateLibroAsync(libro);

        // Assert
        Assert.True(result);
        _mockLibroRepository.Verify(r => r.Update(It.IsAny<Libro>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateLibroAsync_WithInvalidLibro_ReturnsFalse()
    {
        // Arrange
        var libro = new Libro { LibroId = 1 };
        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(false);

        // Act
        var result = await _libroService.UpdateLibroAsync(libro);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.Update(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateLibroAsync_WithNonExistingLibro_ReturnsFalse()
    {
        // Arrange
        var libro = new Libro { LibroId = 1 };
        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(true);
        _mockLibroRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Libro?)null);

        // Act
        var result = await _libroService.UpdateLibroAsync(libro);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.Update(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteLibroAsync_WithNonExistingLibro_ReturnsFalse()
    {
        // Arrange
        _mockLibroRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Libro?)null);

        // Act
        var result = await _libroService.DeleteLibroAsync(1);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.Remove(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ExisteIsbnAsync_WithExistingIsbn_ReturnsTrue()
    {
        // Arrange
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync("1234567890"))
            .ReturnsAsync(true);

        // Act
        var result = await _libroService.ExisteIsbnAsync("1234567890");

        // Assert
        Assert.True(result);
        _mockLibroRepository.Verify(r => r.IsbnExistsAsync("1234567890"), Times.Once);
    }
}