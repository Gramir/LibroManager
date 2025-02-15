using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class AutorServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAutorRepository> _mockAutorRepository;
    private readonly AutorService _autorService;

    public AutorServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAutorRepository = new Mock<IAutorRepository>();
        
        _mockUnitOfWork.Setup(u => u.Autores).Returns(_mockAutorRepository.Object);
        
        _autorService = new AutorService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task CreateAutorAsync_WithValidAutor_ReturnsTrue()
    {
        // Arrange
        var autor = new Autor { Nombre = "Test Autor" };

        // Act
        var result = await _autorService.CreateAutorAsync(autor);

        // Assert
        Assert.True(result);
        _mockAutorRepository.Verify(r => r.AddAsync(It.IsAny<Autor>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAutorAsync_WithEmptyNombre_ReturnsFalse()
    {
        // Arrange
        var autor = new Autor { Nombre = "" };

        // Act
        var result = await _autorService.CreateAutorAsync(autor);

        // Assert
        Assert.False(result);
        _mockAutorRepository.Verify(r => r.AddAsync(It.IsAny<Autor>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAutorAsync_WithExistingLibros_ReturnsFalse()
    {
        // Arrange
        int autorId = 1;
        var autor = new Autor { AutorId = autorId, Nombre = "Test Autor" };

        _mockAutorRepository.Setup(r => r.GetByIdAsync(autorId))
            .ReturnsAsync(autor);
        _mockAutorRepository.Setup(r => r.HasLibrosAsync(autorId))
            .ReturnsAsync(true);

        // Act
        var result = await _autorService.DeleteAutorAsync(autorId);

        // Assert
        Assert.False(result);
        _mockAutorRepository.Verify(r => r.Remove(It.IsAny<Autor>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAutorAsync_WithValidAutor_ReturnsTrue()
    {
        // Arrange
        var autor = new Autor { AutorId = 1, Nombre = "Updated Autor" };
        _mockAutorRepository.Setup(r => r.GetByIdAsync(autor.AutorId))
            .ReturnsAsync(autor);

        // Act
        var result = await _autorService.UpdateAutorAsync(autor);

        // Assert
        Assert.True(result);
        _mockAutorRepository.Verify(r => r.Update(It.IsAny<Autor>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAutoresAsync_ReturnsAllAutores()
    {
        // Arrange
        var autores = new List<Autor>
        {
            new() { AutorId = 1, Nombre = "Autor 1" },
            new() { AutorId = 2, Nombre = "Autor 2" }
        };
        _mockAutorRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(autores);

        // Act
        var result = await _autorService.GetAllAutoresAsync();

        // Assert
        Assert.Equal(autores, result);
        _mockAutorRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAutorByIdAsync_ReturnsAutor()
    {
        // Arrange
        var autor = new Autor { AutorId = 1, Nombre = "Test Autor" };
        _mockAutorRepository.Setup(r => r.GetAutorWithLibrosAsync(1))
            .ReturnsAsync(autor);

        // Act
        var result = await _autorService.GetAutorByIdAsync(1);

        // Assert
        Assert.Equal(autor, result);
        _mockAutorRepository.Verify(r => r.GetAutorWithLibrosAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetAutorByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        _mockAutorRepository.Setup(r => r.GetAutorWithLibrosAsync(1))
            .ReturnsAsync((Autor?)null);

        // Act
        var result = await _autorService.GetAutorByIdAsync(1);

        // Assert
        Assert.Null(result);
        _mockAutorRepository.Verify(r => r.GetAutorWithLibrosAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAutorAsync_WithNonExistingAutor_ReturnsFalse()
    {
        // Arrange
        _mockAutorRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Autor?)null);

        // Act
        var result = await _autorService.DeleteAutorAsync(1);

        // Assert
        Assert.False(result);
        _mockAutorRepository.Verify(r => r.Remove(It.IsAny<Autor>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAutorAsync_WithNoLibros_ReturnsTrue()
    {
        // Arrange
        var autor = new Autor { AutorId = 1, Nombre = "Test Autor" };
        _mockAutorRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(autor);
        _mockAutorRepository.Setup(r => r.HasLibrosAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _autorService.DeleteAutorAsync(1);

        // Assert
        Assert.True(result);
        _mockAutorRepository.Verify(r => r.Remove(autor), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAutorAsync_WithNonExistingAutor_ReturnsFalse()
    {
        // Arrange
        var autor = new Autor { AutorId = 1, Nombre = "Updated Autor" };
        _mockAutorRepository.Setup(r => r.GetByIdAsync(autor.AutorId))
            .ReturnsAsync((Autor?)null);

        // Act
        var result = await _autorService.UpdateAutorAsync(autor);

        // Assert
        Assert.False(result);
        _mockAutorRepository.Verify(r => r.Update(It.IsAny<Autor>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }
}