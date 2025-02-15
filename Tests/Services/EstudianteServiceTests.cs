using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class EstudianteServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly EstudianteService _service;

    public EstudianteServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _service = new EstudianteService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEstudiantes()
    {
        // Arrange
        var expectedEstudiantes = new List<Estudiante>
        {
            new() { EstudianteId = 1, Nombre = "Estudiante 1", Email = "email1@test.com" },
            new() { EstudianteId = 2, Nombre = "Estudiante 2", Email = "email2@test.com" }
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetAllAsync())
            .ReturnsAsync(expectedEstudiantes);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedEstudiantes.Count, result.Count());
    }

    [Fact]
    public async Task GetByEmailAsync_ReturnsEstudiante_WhenEmailExists()
    {
        // Arrange
        var email = "test@test.com";
        var expectedEstudiante = new Estudiante { EstudianteId = 1, Email = email };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(email))
            .ReturnsAsync(expectedEstudiante);

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
        var expectedEstudiantes = new List<Estudiante>
        {
            new() { EstudianteId = 1, Nombre = "Estudiante 1", Email = "email1@test.com" }
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetEstudiantesWithPrestamosActivosAsync())
            .ReturnsAsync(expectedEstudiantes);

        // Act
        var result = await _service.GetEstudiantesWithPrestamosActivosAsync();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsTrue_WhenEmailDoesNotExist()
    {
        // Arrange
        var estudiante = new Estudiante { Email = "new@test.com", Nombre = "Test Name" };
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync((Estudiante?)null);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(estudiante);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.AddAsync(estudiante), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenEmailExists()
    {
        // Arrange
        var estudiante = new Estudiante { Email = "existing@test.com" };
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync(new Estudiante { EstudianteId = 1, Email = estudiante.Email });

        // Act
        var result = await _service.CreateAsync(estudiante);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.AddAsync(estudiante), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenEmailDoesNotExist()
    {
        // Arrange
        var estudiante = new Estudiante { EstudianteId = 1, Email = "update@test.com", Nombre = "Test Name" };
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(estudiante.EstudianteId))
            .ReturnsAsync(estudiante);
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync((Estudiante)null!);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(estudiante);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.Update(estudiante), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenEmailExistsForDifferentEstudiante()
    {
        // Arrange
        var estudiante = new Estudiante { EstudianteId = 1, Email = "existing@test.com" };
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync(new Estudiante { EstudianteId = 2, Email = estudiante.Email });

        // Act
        var result = await _service.UpdateAsync(estudiante);

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
        var estudiante = new Estudiante { 
            Email = "invalid-email",
            Nombre = "Test",
            FechaInscripcion = DateTime.Now 
        };

        // Act
        var result = await _service.CreateAsync(estudiante);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.AddAsync(estudiante), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var estudiante = new Estudiante { 
            Email = "test@test.com",
            Nombre = "Test",
            FechaInscripcion = DateTime.Now 
        };

        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByEmailAsync(estudiante.Email))
            .ReturnsAsync((Estudiante?)null);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _service.CreateAsync(estudiante);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenEmailFormatIsInvalid()
    {
        // Arrange
        var estudiante = new Estudiante { 
            EstudianteId = 1,
            Email = "invalid-email",
            Nombre = "Test",
            FechaInscripcion = DateTime.Now 
        };

        // Act
        var result = await _service.UpdateAsync(estudiante);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Estudiantes.Update(estudiante), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenEstudianteTienePrestamosActivos()
    {
        // Arrange
        var estudianteId = 1;
        var estudiante = new Estudiante { 
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