using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class EstudianteServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<EstudianteService>> _mockLogger;
    private readonly EstudianteService _service;

    public EstudianteServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<EstudianteService>>();
        _service = new EstudianteService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEstudiantes()
    {
        // Arrange
        var estudiantes = new List<Estudiante>
        {
            new()
            {
                EstudianteId = 1,
                Nombre = "Estudiante 1",
                Email = "estudiante1@test.com",
                FechaInscripcion = DateTime.Now
            }
        };

        var estudiantesDto = new List<EstudianteDTO>
        {
            new()
            {
                EstudianteId = 1,
                Nombre = "Estudiante 1",
                Email = "estudiante1@test.com",
                FechaInscripcion = DateTime.Now
            }
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetAllAsync())
            .ReturnsAsync(estudiantes);
        _mockMapper.Setup(m => m.Map<IEnumerable<EstudianteDTO>>(estudiantes))
            .Returns(estudiantesDto);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Estudiante 1", result.First().Nombre);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsEstudiante_WhenEmailExists()
    {
        // Arrange
        var email = "test@test.com";
        var estudiante = new Estudiante
        {
            EstudianteId = 1,
            Nombre = "Test Estudiante",
            Email = email
        };

        var estudianteDto = new EstudianteDTO
        {
            EstudianteId = 1,
            Nombre = "Test Estudiante",
            Email = email
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(email))
            .ReturnsAsync(estudiante);
        _mockMapper.Setup(m => m.Map<EstudianteDTO>(estudiante))
            .Returns(estudianteDto);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetEstudiantesWithPrestamosActivosAsync_ReturnsEstudiantesWithActivePrestamos()
    {
        // Arrange
        var estudiantes = new List<Estudiante>
        {
            new()
            {
                EstudianteId = 1,
                Nombre = "Estudiante 1",
                Prestamos = new List<Prestamo>
                {
                    new() { FechaVencimiento = DateTime.Now.AddDays(7) }
                }
            }
        };

        var estudiantesDto = new List<EstudianteDTO>
        {
            new()
            {
                EstudianteId = 1,
                Nombre = "Estudiante 1",
                PrestamosActivos = 1
            }
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetEstudiantesWithPrestamosActivosAsync())
            .ReturnsAsync(estudiantes);
        _mockMapper.Setup(m => m.Map<IEnumerable<EstudianteDTO>>(estudiantes))
            .Returns(estudiantesDto);

        // Act
        var result = await _service.GetEstudiantesWithPrestamosActivosAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().PrestamosActivos);
    }

    [Fact]
    public async Task CreateAsync_ReturnsTrue_WhenEmailDoesNotExist()
    {
        // Arrange
        var estudianteCreateDto = new EstudianteCreateDTO
        {
            Nombre = "Nuevo Estudiante",
            Email = "nuevo@test.com"
        };

        var estudiante = new Estudiante
        {
            Nombre = "Nuevo Estudiante",
            Email = "nuevo@test.com"
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync((Estudiante?)null);
        _mockMapper.Setup(m => m.Map<Estudiante>(estudianteCreateDto))
            .Returns(estudiante);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(estudianteCreateDto);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.AddAsync(estudiante), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenEmailExists()
    {
        // Arrange
        var estudianteCreateDto = new EstudianteCreateDTO
        {
            Nombre = "Estudiante Existente",
            Email = "existente@test.com"
        };

        var estudiante = new Estudiante
        {
            Nombre = "Estudiante Existente",
            Email = "existente@test.com"
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync(new Estudiante { EstudianteId = 1, Email = estudiante.Email });
        _mockMapper.Setup(m => m.Map<Estudiante>(estudianteCreateDto))
            .Returns(estudiante);

        // Act
        var result = await _service.CreateAsync(estudianteCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.AddAsync(estudiante), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenEmailDoesNotExist()
    {
        // Arrange
        var estudianteUpdateDto = new EstudianteUpdateDTO
        {
            EstudianteId = 1,
            Nombre = "Estudiante Actualizado",
            Email = "actualizado@test.com"
        };

        var estudiante = new Estudiante
        {
            EstudianteId = 1,
            Nombre = "Estudiante Actualizado",
            Email = "actualizado@test.com",
            FechaInscripcion = DateTime.Now
        };

        var existingEstudiante = new Estudiante
        {
            EstudianteId = 1,
            Nombre = "Estudiante Original",
            Email = "original@test.com",
            FechaInscripcion = DateTime.Now
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(estudianteUpdateDto.EstudianteId))
            .ReturnsAsync(existingEstudiante);
        _mockMapper.Setup(m => m.Map<Estudiante>(estudianteUpdateDto))
            .Returns(estudiante);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(estudianteUpdateDto);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.Update(estudiante), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenEmailExistsForDifferentEstudiante()
    {
        // Arrange
        var estudianteUpdateDto = new EstudianteUpdateDTO
        {
            EstudianteId = 1,
            Nombre = "Estudiante Actualizado",
            Email = "existente@test.com"
        };

        var estudiante = new Estudiante
        {
            EstudianteId = 1,
            Nombre = "Estudiante Actualizado",
            Email = "existente@test.com"
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync(new Estudiante { EstudianteId = 2, Email = estudiante.Email });
        _mockMapper.Setup(m => m.Map<Estudiante>(estudianteUpdateDto))
            .Returns(estudiante);

        // Act
        var result = await _service.UpdateAsync(estudianteUpdateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.Update(estudiante), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenEstudianteExists()
    {
        // Arrange
        var estudianteId = 1;
        var estudiante = new Estudiante { EstudianteId = estudianteId };
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(estudianteId))
            .ReturnsAsync(estudiante);
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByEstudianteAsync(estudianteId))
            .ReturnsAsync(new List<Prestamo>());
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAsync(estudianteId);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.Remove(estudiante), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenEstudianteDoesNotExist()
    {
        // Arrange
        var estudianteId = 1;
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(estudianteId))
            .ReturnsAsync((Estudiante?)null);

        // Act
        var result = await _service.DeleteAsync(estudianteId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.Remove(It.IsAny<Estudiante>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenEmailIsInvalid()
    {
        // Arrange
        var estudianteCreateDto = new EstudianteCreateDTO
        {
            Nombre = "Test Estudiante",
            Email = "emailinvalido"
        };

        // Act
        var result = await _service.CreateAsync(estudianteCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.AddAsync(It.IsAny<Estudiante>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var estudianteCreateDto = new EstudianteCreateDTO
        {
            Nombre = "Test Estudiante",
            Email = "test@test.com"
        };

        var estudiante = new Estudiante
        {
            Nombre = "Test Estudiante",
            Email = "test@test.com"
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync((Estudiante?)null);
        _mockMapper.Setup(m => m.Map<Estudiante>(estudianteCreateDto))
            .Returns(estudiante);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _service.CreateAsync(estudianteCreateDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenEmailFormatIsInvalid()
    {
        // Arrange
        var estudianteUpdateDto = new EstudianteUpdateDTO
        {
            EstudianteId = 1,
            Nombre = "Test Estudiante",
            Email = "emailinvalido"
        };

        // Act
        var result = await _service.UpdateAsync(estudianteUpdateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.Update(It.IsAny<Estudiante>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenEstudianteTienePrestamosActivos()
    {
        // Arrange
        var estudianteId = 1;
        var estudiante = new Estudiante
        {
            EstudianteId = estudianteId,
            Prestamos = new List<Prestamo> {
                new() {
                    PrestamoId = 1,
                    FechaVencimiento = DateTime.Now.AddDays(5)
                }
            }
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(estudianteId))
            .ReturnsAsync(estudiante);

        // Act
        var result = await _service.DeleteAsync(estudianteId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.Remove(estudiante), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsNull_WhenDatabaseError()
    {
        // Arrange
        var email = "test@test.com";
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(email))
            .ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        Assert.Null(result);
    }
}