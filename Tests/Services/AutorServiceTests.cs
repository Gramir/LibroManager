using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using AutoMapper;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class AutorServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAutorRepository> _mockAutorRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly AutorService _autorService;

    public AutorServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAutorRepository = new Mock<IAutorRepository>();
        _mockMapper = new Mock<IMapper>();
        
        _mockUnitOfWork.Setup(u => u.Autores).Returns(_mockAutorRepository.Object);
        _autorService = new AutorService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAutores()
    {
        // Arrange
        var autores = new List<Autor>
        {
            new() { AutorId = 1, Nombre = "Autor 1" },
            new() { AutorId = 2, Nombre = "Autor 2" }
        };

        var autoresDto = new List<AutorDTO>
        {
            new() { AutorId = 1, Nombre = "Autor 1" },
            new() { AutorId = 2, Nombre = "Autor 2" }
        };

        _mockAutorRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(autores);
        _mockMapper.Setup(m => m.Map<IEnumerable<AutorDTO>>(autores))
            .Returns(autoresDto);

        // Act
        var result = await _autorService.GetAllAsync();

        // Assert
        Assert.Equal(autoresDto.Count, result.Count());
        _mockAutorRepository.Verify(r => r.GetAllAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<AutorDTO>>(autores), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsAutor()
    {
        // Arrange
        var autor = new Autor { AutorId = 1, Nombre = "Test Autor" };
        var autorDto = new AutorDTO { AutorId = 1, Nombre = "Test Autor" };

        _mockAutorRepository.Setup(r => r.GetAutorWithLibrosAsync(1))
            .ReturnsAsync(autor);
        _mockMapper.Setup(m => m.Map<AutorDTO>(autor))
            .Returns(autorDto);

        // Act
        var result = await _autorService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(autorDto.Nombre, result.Nombre);
        _mockAutorRepository.Verify(r => r.GetAutorWithLibrosAsync(1), Times.Once);
        _mockMapper.Verify(m => m.Map<AutorDTO>(autor), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidAutor_ReturnsTrue()
    {
        // Arrange
        var autorCreateDto = new AutorCreateDTO { Nombre = "Test Autor" };
        var autor = new Autor { Nombre = "Test Autor" };

        _mockMapper.Setup(m => m.Map<Autor>(autorCreateDto))
            .Returns(autor);

        // Act
        var result = await _autorService.CreateAsync(autorCreateDto);

        // Assert
        Assert.True(result);
        _mockAutorRepository.Verify(r => r.AddAsync(It.IsAny<Autor>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyNombre_ReturnsFalse()
    {
        // Arrange
        var autorCreateDto = new AutorCreateDTO { Nombre = "" };

        // Act
        var result = await _autorService.CreateAsync(autorCreateDto);

        // Assert
        Assert.False(result);
        _mockAutorRepository.Verify(r => r.AddAsync(It.IsAny<Autor>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WithValidAutor_ReturnsTrue()
    {
        // Arrange
        var autorUpdateDto = new AutorUpdateDTO { AutorId = 1, Nombre = "Updated Autor" };
        var autor = new Autor { AutorId = 1, Nombre = "Updated Autor" };

        _mockMapper.Setup(m => m.Map<Autor>(autorUpdateDto))
            .Returns(autor);
        _mockAutorRepository.Setup(r => r.GetByIdAsync(autor.AutorId))
            .ReturnsAsync(autor);

        // Act
        var result = await _autorService.UpdateAsync(autorUpdateDto);

        // Assert
        Assert.True(result);
        _mockAutorRepository.Verify(r => r.Update(It.IsAny<Autor>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingAutor_ReturnsFalse()
    {
        // Arrange
        var autorUpdateDto = new AutorUpdateDTO { AutorId = 1, Nombre = "Updated Autor" };
        var autor = new Autor { AutorId = 1, Nombre = "Updated Autor" };

        _mockMapper.Setup(m => m.Map<Autor>(autorUpdateDto))
            .Returns(autor);
        _mockAutorRepository.Setup(r => r.GetByIdAsync(autor.AutorId))
            .ReturnsAsync((Autor?)null);

        // Act
        var result = await _autorService.UpdateAsync(autorUpdateDto);

        // Assert
        Assert.False(result);
        _mockAutorRepository.Verify(r => r.Update(It.IsAny<Autor>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingAutor_ReturnsFalse()
    {
        // Arrange
        _mockAutorRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Autor?)null);

        // Act
        var result = await _autorService.DeleteAsync(1);

        // Assert
        Assert.False(result);
        _mockAutorRepository.Verify(r => r.Remove(It.IsAny<Autor>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingLibros_ReturnsFalse()
    {
        // Arrange
        var autor = new Autor { AutorId = 1, Nombre = "Test Autor" };

        _mockAutorRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(autor);
        _mockAutorRepository.Setup(r => r.HasLibrosAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _autorService.DeleteAsync(1);

        // Assert
        Assert.False(result);
        _mockAutorRepository.Verify(r => r.Remove(It.IsAny<Autor>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithNoLibros_ReturnsTrue()
    {
        // Arrange
        var autor = new Autor { AutorId = 1, Nombre = "Test Autor" };

        _mockAutorRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(autor);
        _mockAutorRepository.Setup(r => r.HasLibrosAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _autorService.DeleteAsync(1);

        // Assert
        Assert.True(result);
        _mockAutorRepository.Verify(r => r.Remove(autor), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}