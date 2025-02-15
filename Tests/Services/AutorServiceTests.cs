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
}