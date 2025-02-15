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
}