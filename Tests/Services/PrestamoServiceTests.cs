using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class PrestamoServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly PrestamoService _service;

    public PrestamoServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _service = new PrestamoService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPrestamos()
    {
        // Arrange
        var expectedPrestamos = new List<Prestamo>
        {
            new() { PrestamoId = 1, LibroId = 1, EstudianteId = 1 },
            new() { PrestamoId = 2, LibroId = 2, EstudianteId = 2 }
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetAllAsync())
            .ReturnsAsync(expectedPrestamos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedPrestamos.Count, result.Count());
    }

    [Fact]
    public async Task GetPrestamosByEstudianteAsync_ReturnsPrestamosForEstudiante()
    {
        // Arrange
        var estudianteId = 1;
        var expectedPrestamos = new List<Prestamo>
        {
            new() { PrestamoId = 1, LibroId = 1, EstudianteId = estudianteId }
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByEstudianteAsync(estudianteId))
            .ReturnsAsync(expectedPrestamos);

        // Act
        var result = await _service.GetPrestamosByEstudianteAsync(estudianteId);

        // Assert
        Assert.Single(result);
        Assert.Equal(estudianteId, result.First().EstudianteId);
    }

    [Fact]
    public async Task GetPrestamosByLibroAsync_ReturnsPrestamosForLibro()
    {
        // Arrange
        var libroId = 1;
        var expectedPrestamos = new List<Prestamo>
        {
            new() { PrestamoId = 1, LibroId = libroId, EstudianteId = 1 }
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(libroId))
            .ReturnsAsync(expectedPrestamos);

        // Act
        var result = await _service.GetPrestamosByLibroAsync(libroId);

        // Assert
        Assert.Single(result);
        Assert.Equal(libroId, result.First().LibroId);
    }

    [Fact]
    public async Task CreateAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var prestamo = new Prestamo { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Mock libro exists
        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(new Libro { LibroId = prestamo.LibroId });

        // Mock libro no está prestado
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId))
            .ReturnsAsync(new List<Prestamo>());

        // Mock estudiante exists
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(prestamo.EstudianteId))
            .ReturnsAsync(new Estudiante { EstudianteId = prestamo.EstudianteId });

        // Mock estudiante no tiene préstamos vencidos
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId))
            .ReturnsAsync(new List<Prestamo>());

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenException()
    {
        // Arrange
        var prestamo = new Prestamo { LibroId = 1, EstudianteId = 1 };
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ThrowsAsync(new Exception());

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var prestamo = new Prestamo { 
            PrestamoId = 1, 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Mock existing prestamo
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync(new Prestamo { 
                PrestamoId = prestamo.PrestamoId, 
                LibroId = prestamo.LibroId 
            });

        // Mock no hay préstamos activos para el libro
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId))
            .ReturnsAsync(new List<Prestamo>());

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(prestamo);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Update(prestamo), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenPrestamoExists()
    {
        // Arrange
        var prestamoId = 1;
        var prestamo = new Prestamo { PrestamoId = prestamoId };
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamoId))
            .ReturnsAsync(prestamo);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAsync(prestamoId);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Remove(prestamo), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenPrestamoDoesNotExist()
    {
        // Arrange
        var prestamoId = 1;
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamoId))
            .ReturnsAsync((Prestamo?)null);

        // Act
        var result = await _service.DeleteAsync(prestamoId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Remove(It.IsAny<Prestamo>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenLibroDoesNotExist()
    {
        // Arrange
        var prestamo = new Prestamo { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Mock libro no existe
        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync((Libro?)null);

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenLibroYaEstaPrestado()
    {
        // Arrange
        var prestamo = new Prestamo { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Mock libro existe
        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(new Libro { LibroId = prestamo.LibroId });

        // Mock libro ya está prestado
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId))
            .ReturnsAsync(new List<Prestamo> 
            { 
                new() { 
                    LibroId = prestamo.LibroId, 
                    FechaVencimiento = DateTime.Now.AddDays(5) 
                } 
            });

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenEstudianteDoesNotExist()
    {
        // Arrange
        var prestamo = new Prestamo { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Mock libro existe y está disponible
        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(new Libro { LibroId = prestamo.LibroId });
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId))
            .ReturnsAsync(new List<Prestamo>());

        // Mock estudiante no existe
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(prestamo.EstudianteId))
            .ReturnsAsync((Estudiante?)null);

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenEstudianteTienePrestamosVencidos()
    {
        // Arrange
        var prestamo = new Prestamo { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Mock libro existe y está disponible
        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(new Libro { LibroId = prestamo.LibroId });
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId))
            .ReturnsAsync(new List<Prestamo>());

        // Mock estudiante existe
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(prestamo.EstudianteId))
            .ReturnsAsync(new Estudiante { EstudianteId = prestamo.EstudianteId });

        // Mock estudiante tiene préstamos vencidos
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId))
            .ReturnsAsync(new List<Prestamo> 
            { 
                new() { 
                    EstudianteId = prestamo.EstudianteId, 
                    FechaVencimiento = DateTime.Now.AddDays(-1) 
                } 
            });

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenPrestamoNoExiste()
    {
        // Arrange
        var prestamo = new Prestamo { 
            PrestamoId = 1, 
            LibroId = 1, 
            EstudianteId = 1 
        };

        // Mock prestamo no existe
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync((Prestamo?)null);

        // Act
        var result = await _service.UpdateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Update(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenNuevoLibroYaEstaPrestado()
    {
        // Arrange
        var prestamo = new Prestamo { 
            PrestamoId = 1, 
            LibroId = 2,  // Nuevo libro diferente al original
            EstudianteId = 1 
        };

        // Mock prestamo existe con libro diferente
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync(new Prestamo { 
                PrestamoId = prestamo.PrestamoId, 
                LibroId = 1  // Libro original
            });

        // Mock nuevo libro ya está prestado
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId))
            .ReturnsAsync(new List<Prestamo> 
            { 
                new() { 
                    LibroId = prestamo.LibroId, 
                    FechaVencimiento = DateTime.Now.AddDays(5) 
                } 
            });

        // Act
        var result = await _service.UpdateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Update(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenException()
    {
        // Arrange
        var prestamoId = 1;
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamoId))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _service.GetByIdAsync(prestamoId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPrestamosActivosAsync_ReturnsEmpty_WhenException()
    {
        // Arrange
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosActivosAsync())
            .ThrowsAsync(new Exception());

        // Act
        var result = await _service.GetPrestamosActivosAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenFechaPrestamoPosteriorAVencimiento()
    {
        // Arrange
        var prestamo = new Prestamo 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now.AddDays(7),
            FechaVencimiento = DateTime.Now.AddDays(1)
        };

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenFechaPrestamoEsFutura()
    {
        // Arrange
        var prestamo = new Prestamo 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now.AddDays(1),
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenFechaVencimientoEsPasada()
    {
        // Arrange
        var prestamo = new Prestamo 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now.AddDays(-7),
            FechaVencimiento = DateTime.Now.AddDays(-1)
        };

        // Act
        var result = await _service.CreateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenFechasInvalidas()
    {
        // Arrange
        var prestamo = new Prestamo 
        { 
            PrestamoId = 1,
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now.AddDays(1),
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync(new Prestamo { PrestamoId = prestamo.PrestamoId });

        // Act
        var result = await _service.UpdateAsync(prestamo);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Update(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenException()
    {
        // Arrange
        var prestamo = new Prestamo 
        { 
            PrestamoId = 1,
            LibroId = 1, 
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync(new Prestamo { PrestamoId = prestamo.PrestamoId });
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ThrowsAsync(new Exception());

        // Act
        var result = await _service.UpdateAsync(prestamo);

        // Assert
        Assert.False(result);
    }
}