using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class UbicacionServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IUbicacionRepository> _mockUbicacionRepository;
    private readonly Mock<ILibroRepository> _mockLibroRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UbicacionService>> _mockLogger;
    private readonly UbicacionService _ubicacionService;

    public UbicacionServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUbicacionRepository = new Mock<IUbicacionRepository>();
        _mockLibroRepository = new Mock<ILibroRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UbicacionService>>();

        _mockUnitOfWork.Setup(u => u.Ubicaciones).Returns(_mockUbicacionRepository.Object);
        _mockUnitOfWork.Setup(u => u.Libros).Returns(_mockLibroRepository.Object);

        _ubicacionService = new UbicacionService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllUbicacionesAsync_ReturnsAllUbicaciones()
    {
        // Arrange
        var ubicaciones = new List<Ubicacion>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1, Libros = new List<Libro>() { new Libro() } },
            new() { UbicacionId = 2, Estante = "B", Nivel = 2, Posicion = 2, Libros = null }
        };

        var ubicacionesDto = new List<UbicacionDTO>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 },
            new() { UbicacionId = 2, Estante = "B", Nivel = 2, Posicion = 2 }
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetAllAsync())
            .ReturnsAsync(ubicaciones);
        _mockMapper.Setup(m => m.Map<IEnumerable<UbicacionDTO>>(It.IsAny<IEnumerable<Ubicacion>>()))
            .Returns(ubicacionesDto);

        // Act
        var result = await _ubicacionService.GetAllUbicacionesAsync();

        // Assert
        Assert.Equal(ubicacionesDto.Count, result.Count());
        Assert.False(result.First().EstaDisponible);
        Assert.True(result.Last().EstaDisponible);
    }

    [Fact]
    public async Task GetAllUbicacionesAsync_ReturnsEmptyList_WhenExceptionOccurs()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetAllAsync())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.GetAllUbicacionesAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUbicacionByIdAsync_ReturnsUbicacion_WhenIdExists()
    {
        // Arrange
        var ubicacionId = 1;
        var ubicacion = new Ubicacion { UbicacionId = ubicacionId, Estante = "A", Nivel = 1, Posicion = 1, Libros = new List<Libro>() };
        var ubicacionDto = new UbicacionDTO { UbicacionId = ubicacionId, Estante = "A", Nivel = 1, Posicion = 1 };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync(ubicacion);
        _mockMapper.Setup(m => m.Map<UbicacionDTO>(ubicacion))
            .Returns(ubicacionDto);

        // Act
        var result = await _ubicacionService.GetUbicacionByIdAsync(ubicacionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ubicacionId, result.UbicacionId);
        Assert.True(result.EstaDisponible);
    }

    [Fact]
    public async Task GetUbicacionByIdAsync_ReturnsNull_WhenIdDoesNotExist()
    {
        // Arrange
        var ubicacionId = 99;
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync((Ubicacion?)null);

        // Act
        var result = await _ubicacionService.GetUbicacionByIdAsync(ubicacionId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUbicacionByIdAsync_ReturnsNull_WhenExceptionOccurs()
    {
        // Arrange
        var ubicacionId = 1;
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.GetUbicacionByIdAsync(ubicacionId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUbicacionesByEstanteAsync_ReturnsFilteredUbicaciones()
    {
        // Arrange
        var estante = "A";
        var ubicaciones = new List<Ubicacion>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1, Libros = new List<Libro>() },
            new() { UbicacionId = 2, Estante = "A", Nivel = 2, Posicion = 2, Libros = null },
            new() { UbicacionId = 3, Estante = "B", Nivel = 1, Posicion = 1, Libros = null }
        };

        var ubicacionesDto = new List<UbicacionDTO>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 },
            new() { UbicacionId = 2, Estante = "A", Nivel = 2, Posicion = 2 }
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetAllAsync())
            .ReturnsAsync(ubicaciones);
        _mockMapper.Setup(m => m.Map<IEnumerable<UbicacionDTO>>(It.IsAny<IEnumerable<Ubicacion>>()))
            .Returns(ubicacionesDto);

        // Act
        var result = await _ubicacionService.GetUbicacionesByEstanteAsync(estante);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.Equal(estante, u.Estante));
    }

    [Fact]
    public async Task GetUbicacionesByEstanteAsync_ReturnsEmptyList_WhenExceptionOccurs()
    {
        // Arrange
        var estante = "A";
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetAllAsync())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.GetUbicacionesByEstanteAsync(estante);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUbicacionesByEstanteAndNivelAsync_ReturnsFilteredUbicaciones()
    {
        // Arrange
        var estante = "A";
        var nivel = 1;
        var ubicaciones = new List<Ubicacion>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1, Libros = new List<Libro>() },
            new() { UbicacionId = 2, Estante = "A", Nivel = 1, Posicion = 2, Libros = null },
            new() { UbicacionId = 3, Estante = "A", Nivel = 2, Posicion = 1, Libros = null },
            new() { UbicacionId = 4, Estante = "B", Nivel = 1, Posicion = 1, Libros = null }
        };

        var ubicacionesFiltradas = ubicaciones.Where(u =>
            u.Estante.Equals(estante, StringComparison.OrdinalIgnoreCase) &&
            u.Nivel == nivel);

        var ubicacionesDto = new List<UbicacionDTO>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 },
            new() { UbicacionId = 2, Estante = "A", Nivel = 1, Posicion = 2 }
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetAllAsync())
            .ReturnsAsync(ubicaciones);
        _mockMapper.Setup(m => m.Map<IEnumerable<UbicacionDTO>>(It.IsAny<IEnumerable<Ubicacion>>()))
            .Returns(ubicacionesDto);

        // Act
        var result = await _ubicacionService.GetUbicacionesByEstanteAndNivelAsync(estante, nivel);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.Equal(estante, u.Estante));
        Assert.All(result, u => Assert.Equal(nivel, u.Nivel));
    }

    [Fact]
    public async Task GetUbicacionesByEstanteAndNivelAsync_ReturnsEmptyList_WhenExceptionOccurs()
    {
        // Arrange
        var estante = "A";
        var nivel = 1;
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetAllAsync())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.GetUbicacionesByEstanteAndNivelAsync(estante, nivel);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAvailableUbicacionesAsync_ReturnsOnlyAvailableUbicaciones()
    {
        // Arrange
        var ubicaciones = new List<Ubicacion>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1, Libros = new List<Libro>() { new Libro() } },
            new() { UbicacionId = 2, Estante = "A", Nivel = 2, Posicion = 2, Libros = new List<Libro>() },
            new() { UbicacionId = 3, Estante = "B", Nivel = 1, Posicion = 1, Libros = null },
            new() { UbicacionId = 4, Estante = "B", Nivel = 2, Posicion = 2, Libros = new List<Libro>() }
        };

        var ubicacionesDisponibles = ubicaciones.Where(u => u.Libros == null || !u.Libros.Any());
        var ubicacionesDto = new List<UbicacionDTO>
        {
            new() { UbicacionId = 3, Estante = "B", Nivel = 1, Posicion = 1 },
            new() { UbicacionId = 4, Estante = "B", Nivel = 2, Posicion = 2 }
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetAllAsync())
            .ReturnsAsync(ubicaciones);
        _mockMapper.Setup(m => m.Map<IEnumerable<UbicacionDTO>>(It.IsAny<IEnumerable<Ubicacion>>()))
            .Returns(ubicacionesDto);

        // Act
        var result = await _ubicacionService.GetAvailableUbicacionesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, u => Assert.True(u.EstaDisponible));
    }

    [Fact]
    public async Task GetAvailableUbicacionesAsync_ReturnsEmptyList_WhenExceptionOccurs()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetAllAsync())
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.GetAvailableUbicacionesAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateUbicacionAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var ubicacionCreateDto = new UbicacionCreateDTO
        {
            Estante = "A",
            Nivel = 1,
            Posicion = 1
        };

        var ubicacion = new Ubicacion
        {
            Estante = "A",
            Nivel = 1,
            Posicion = 1
        };

        _mockMapper.Setup(m => m.Map<Ubicacion>(ubicacionCreateDto))
            .Returns(ubicacion);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _ubicacionService.CreateUbicacionAsync(ubicacionCreateDto);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Ubicaciones.AddAsync(ubicacion), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateUbicacionAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var ubicacionCreateDto = new UbicacionCreateDTO
        {
            Estante = "A",
            Nivel = 1,
            Posicion = 1
        };

        var ubicacion = new Ubicacion
        {
            Estante = "A",
            Nivel = 1,
            Posicion = 1
        };

        _mockMapper.Setup(m => m.Map<Ubicacion>(ubicacionCreateDto))
            .Returns(ubicacion);
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.AddAsync(ubicacion))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.CreateUbicacionAsync(ubicacionCreateDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateUbicacionAsync_ReturnsTrue_WhenUbicacionExists()
    {
        // Arrange
        var ubicacionId = 1;
        var ubicacionUpdateDto = new UbicacionUpdateDTO
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 2
        };

        var ubicacionExistente = new Ubicacion
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 1
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync(ubicacionExistente);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _ubicacionService.UpdateUbicacionAsync(ubicacionId, ubicacionUpdateDto);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Ubicaciones.Update(ubicacionExistente), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUbicacionAsync_ReturnsFalse_WhenUbicacionDoesNotExist()
    {
        // Arrange
        var ubicacionId = 99;
        var ubicacionUpdateDto = new UbicacionUpdateDTO
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 2
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync((Ubicacion?)null);

        // Act
        var result = await _ubicacionService.UpdateUbicacionAsync(ubicacionId, ubicacionUpdateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Ubicaciones.Update(It.IsAny<Ubicacion>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateUbicacionAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var ubicacionId = 1;
        var ubicacionUpdateDto = new UbicacionUpdateDTO
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 2
        };

        var ubicacionExistente = new Ubicacion
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 1
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync(ubicacionExistente);
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.Update(ubicacionExistente))
            .Throws(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.UpdateUbicacionAsync(ubicacionId, ubicacionUpdateDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUbicacionAsync_ReturnsTrue_WhenUbicacionExistsAndHasNoBooks()
    {
        // Arrange
        var ubicacionId = 1;
        var ubicacion = new Ubicacion
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 1,
            Libros = new List<Libro>()
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync(ubicacion);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _ubicacionService.DeleteUbicacionAsync(ubicacionId);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Ubicaciones.Remove(ubicacion), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteUbicacionAsync_ReturnsFalse_WhenUbicacionDoesNotExist()
    {
        // Arrange
        var ubicacionId = 99;

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync((Ubicacion?)null);

        // Act
        var result = await _ubicacionService.DeleteUbicacionAsync(ubicacionId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Ubicaciones.Remove(It.IsAny<Ubicacion>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteUbicacionAsync_ReturnsFalse_WhenUbicacionHasBooks()
    {
        // Arrange
        var ubicacionId = 1;
        var ubicacion = new Ubicacion
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 1,
            Libros = new List<Libro> { new Libro() }
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync(ubicacion);

        // Act
        var result = await _ubicacionService.DeleteUbicacionAsync(ubicacionId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Ubicaciones.Remove(It.IsAny<Ubicacion>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteUbicacionAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var ubicacionId = 1;
        var ubicacion = new Ubicacion
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 1,
            Libros = new List<Libro>()
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync(ubicacion);
        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.Remove(ubicacion))
            .Throws(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.DeleteUbicacionAsync(ubicacionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UbicacionExistsAsync_ReturnsTrue_WhenUbicacionExists()
    {
        // Arrange
        var ubicacionId = 1;
        var ubicacion = new Ubicacion
        {
            UbicacionId = ubicacionId,
            Estante = "A",
            Nivel = 1,
            Posicion = 1
        };

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync(ubicacion);

        // Act
        var result = await _ubicacionService.UbicacionExistsAsync(ubicacionId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UbicacionExistsAsync_ReturnsFalse_WhenUbicacionDoesNotExist()
    {
        // Arrange
        var ubicacionId = 99;

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ReturnsAsync((Ubicacion?)null);

        // Act
        var result = await _ubicacionService.UbicacionExistsAsync(ubicacionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UbicacionExistsAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var ubicacionId = 1;

        _mockUnitOfWork.Setup(uow => uow.Ubicaciones.GetByIdAsync(ubicacionId))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _ubicacionService.UbicacionExistsAsync(ubicacionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAvailableUbicacionesWithCurrentAsync_IncludesCurrent()
    {
        // Arrange
        var ubicaciones = new List<Ubicacion>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 },
            new() { UbicacionId = 2, Estante = "A", Nivel = 1, Posicion = 2 },
            new() { UbicacionId = 3, Estante = "A", Nivel = 1, Posicion = 3 }
        };

        var libros = new List<Libro>
        {
            new() { LibroId = 1, UbicacionId = 1 },
            new() { LibroId = 2, UbicacionId = 2 }
        };

        var ubicacionesDto = new List<UbicacionDTO>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 },
            new() { UbicacionId = 3, Estante = "A", Nivel = 1, Posicion = 3 }
        };

        _mockUbicacionRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(ubicaciones);
        _mockLibroRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(libros);
        _mockMapper.Setup(m => m.Map<List<UbicacionDTO>>(It.IsAny<List<Ubicacion>>()))
            .Returns(ubicacionesDto);

        // Act
        var result = await _ubicacionService.GetAvailableUbicacionesWithCurrentAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.UbicacionId == 1);
        Assert.Contains(result, u => u.UbicacionId == 3);
        Assert.DoesNotContain(result, u => u.UbicacionId == 2);
    }
}