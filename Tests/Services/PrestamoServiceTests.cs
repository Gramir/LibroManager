using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using AutoMapper;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class PrestamoServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly PrestamoService _service;

    public PrestamoServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _service = new PrestamoService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPrestamos()
    {
        // Arrange
        var prestamos = new List<Prestamo>
        {
            new() { 
                PrestamoId = 1, 
                LibroId = 1, 
                EstudianteId = 1,
                Libro = new Libro { Titulo = "Libro 1" },
                Estudiante = new Estudiante { Nombre = "Estudiante 1" }
            }
        };

        var prestamosDto = new List<PrestamoDTO>
        {
            new() { 
                PrestamoId = 1,
                LibroTitulo = "Libro 1",
                EstudianteNombre = "Estudiante 1"
            }
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetAllAsync())
            .ReturnsAsync(prestamos);
        _mockMapper.Setup(m => m.Map<IEnumerable<PrestamoDTO>>(prestamos))
            .Returns(prestamosDto);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(prestamosDto.Count, result.Count());
    }

    [Fact]
    public async Task GetPrestamosByEstudianteAsync_ReturnsPrestamosForEstudiante()
    {
        // Arrange
        var estudianteId = 1;
        var prestamos = new List<Prestamo>
        {
            new() { 
                PrestamoId = 1, 
                LibroId = 1, 
                EstudianteId = estudianteId,
                Libro = new Libro { Titulo = "Libro 1" },
                Estudiante = new Estudiante { Nombre = "Estudiante 1" }
            }
        };

        var prestamosDto = new List<PrestamoDTO>
        {
            new() { 
                PrestamoId = 1,
                LibroTitulo = "Libro 1",
                EstudianteNombre = "Estudiante 1"
            }
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByEstudianteAsync(estudianteId))
            .ReturnsAsync(prestamos);
        _mockMapper.Setup(m => m.Map<IEnumerable<PrestamoDTO>>(prestamos))
            .Returns(prestamosDto);

        // Act
        var result = await _service.GetPrestamosByEstudianteAsync(estudianteId);

        // Assert
        Assert.Single(result);
        Assert.Equal("Libro 1", result.First().LibroTitulo);
    }

    [Fact]
    public async Task GetPrestamosByLibroAsync_ReturnsPrestamosForLibro()
    {
        // Arrange
        var libroId = 1;
        var prestamos = new List<Prestamo>
        {
            new() { 
                PrestamoId = 1, 
                LibroId = libroId, 
                EstudianteId = 1,
                Libro = new Libro { Titulo = "Libro 1" },
                Estudiante = new Estudiante { Nombre = "Estudiante 1" }
            }
        };

        var prestamosDto = new List<PrestamoDTO>
        {
            new() { 
                PrestamoId = 1,
                LibroTitulo = "Libro 1",
                EstudianteNombre = "Estudiante 1"
            }
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(libroId))
            .ReturnsAsync(prestamos);
        _mockMapper.Setup(m => m.Map<IEnumerable<PrestamoDTO>>(prestamos))
            .Returns(prestamosDto);

        // Act
        var result = await _service.GetPrestamosByLibroAsync(libroId);

        // Assert
        Assert.Single(result);
        Assert.Equal("Libro 1", result.First().LibroTitulo);
    }

    [Fact]
    public async Task CreateAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo
        {
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

        // Mock estudiante no tiene préstamos vencidos
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId))
            .ReturnsAsync(new List<Prestamo>());

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenException()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo
        {
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ThrowsAsync(new Exception());

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenSuccessful()
    {
        // Arrange
        var prestamoUpdateDto = new PrestamoUpdateDTO 
        { 
            PrestamoId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo 
        { 
            PrestamoId = 1,
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var libro = new Libro { LibroId = 1, Estado = EstadoLibro.Prestado };

        // Mock existing prestamo
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync(new Prestamo { 
                PrestamoId = prestamo.PrestamoId,
                LibroId = 1,
                EstudianteId = 1,
                FechaPrestamo = DateTime.Now
            });

        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(libro);

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoUpdateDto))
            .Returns(prestamo);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(prestamoUpdateDto);

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
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo
        {
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Mock libro no existe
        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync((Libro?)null);

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenLibroYaEstaPrestado()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo
        {
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

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenEstudianteDoesNotExist()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo
        {
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

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenEstudianteTienePrestamosVencidos()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo
        {
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
                    FechaVencimiento = DateTime.Now.AddDays(-1),
                    Estado = EstadoPrestamo.Expirado
                } 
            });

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenPrestamoNoExiste()
    {
        // Arrange
        var prestamoUpdateDto = new PrestamoUpdateDTO 
        { 
            PrestamoId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo 
        { 
            PrestamoId = 1,
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        // Mock prestamo no existe
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync((Prestamo?)null);

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoUpdateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.UpdateAsync(prestamoUpdateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Update(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenNuevoLibroYaEstaPrestado()
    {
        // Arrange
        var prestamoUpdateDto = new PrestamoUpdateDTO 
        { 
            PrestamoId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo 
        { 
            PrestamoId = 1,
            LibroId = 2,  // Nuevo libro diferente al original
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
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

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoUpdateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.UpdateAsync(prestamoUpdateDto);

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
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(-1)
        };

        var prestamo = new Prestamo
        {
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(-1)
        };

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenFechaPrestamoEsFutura()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo
        {
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now.AddDays(1),
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenFechaVencimientoEsPasada()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(-1)
        };

        var prestamo = new Prestamo
        {
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now.AddDays(-7),
            FechaVencimiento = DateTime.Now.AddDays(-1)
        };

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenFechasInvalidas()
    {
        // Arrange
        var prestamoUpdateDto = new PrestamoUpdateDTO 
        { 
            PrestamoId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

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

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoUpdateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.UpdateAsync(prestamoUpdateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Update(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenException()
    {
        // Arrange
        var prestamoUpdateDto = new PrestamoUpdateDTO 
        { 
            PrestamoId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

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

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoUpdateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.UpdateAsync(prestamoUpdateDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenFechaVencimientoMasDe30Dias()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(31)
        };

        var prestamo = new Prestamo
        {
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(31)
        };

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenFechaVencimientoMasDe30Dias()
    {
        // Arrange
        var prestamoUpdateDto = new PrestamoUpdateDTO 
        { 
            PrestamoId = 1,
            FechaVencimiento = DateTime.Now.AddDays(31)
        };

        var prestamo = new Prestamo 
        { 
            PrestamoId = 1,
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(31)
        };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync(new Prestamo { 
                PrestamoId = prestamo.PrestamoId,
                LibroId = 1,
                EstudianteId = 1,
                FechaPrestamo = DateTime.Now
            });

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoUpdateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.UpdateAsync(prestamoUpdateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.Update(prestamo), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(15)]
    [InlineData(30)]
    public async Task CreateAsync_ReturnsTrue_WhenFechaVencimientoValida(int dias)
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(dias)
        };

        var prestamo = new Prestamo
        {
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(dias)
        };

        // Mock libro existe y está disponible
        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(new Libro { LibroId = prestamo.LibroId });
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId))
            .ReturnsAsync(new List<Prestamo>());

        // Mock estudiante existe
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(prestamo.EstudianteId))
            .ReturnsAsync(new Estudiante { EstudianteId = prestamo.EstudianteId });

        // Mock estudiante no tiene préstamos vencidos
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId))
            .ReturnsAsync(new List<Prestamo>());

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Prestamos.AddAsync(prestamo), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_MarcarLibroPerdido_CuandoPrestamoExpira()
    {
        // Arrange
        var prestamoUpdateDto = new PrestamoUpdateDTO 
        { 
            PrestamoId = 1,
            FechaVencimiento = DateTime.Now.AddDays(-1)  // Fecha vencida
        };

        var prestamo = new Prestamo 
        { 
            PrestamoId = 1,
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now.AddDays(-7),
            FechaVencimiento = DateTime.Now.AddDays(-1)
        };

        var libro = new Libro { LibroId = 1, Estado = EstadoLibro.Prestado };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync(new Prestamo { 
                PrestamoId = prestamo.PrestamoId,
                LibroId = 1,
                EstudianteId = 1,
                FechaPrestamo = DateTime.Now.AddDays(-7)
            });

        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(libro);

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoUpdateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.UpdateAsync(prestamoUpdateDto);

        // Assert
        Assert.True(result);
        Assert.Equal(EstadoPrestamo.Expirado, prestamo.Estado);
        Assert.Equal(EstadoLibro.Perdido, libro.Estado);
        _mockUnitOfWork.Verify(uow => uow.Libros.Update(libro), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_MarcarLibroDisponible_CuandoPrestamoDevuelto()
    {
        // Arrange
        var prestamoUpdateDto = new PrestamoUpdateDTO 
        { 
            PrestamoId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo 
        { 
            PrestamoId = 1,
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7),
            Estado = EstadoPrestamo.Concluido
        };

        var libro = new Libro { LibroId = 1, Estado = EstadoLibro.Prestado };

        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetByIdAsync(prestamo.PrestamoId))
            .ReturnsAsync(new Prestamo { 
                PrestamoId = prestamo.PrestamoId,
                LibroId = 1,
                EstudianteId = 1,
                FechaPrestamo = DateTime.Now
            });

        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(libro);

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoUpdateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.UpdateAsync(prestamoUpdateDto);

        // Assert
        Assert.True(result);
        Assert.Equal(EstadoPrestamo.Concluido, prestamo.Estado);
        Assert.Equal(EstadoLibro.Disponible, libro.Estado);
        Assert.NotNull(prestamo.FechaDevolucion);
        _mockUnitOfWork.Verify(uow => uow.Libros.Update(libro), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ActualizaEstadoLibro_CuandoPrestamoCreado()
    {
        // Arrange
        var prestamoCreateDto = new PrestamoCreateDTO 
        { 
            LibroId = 1, 
            EstudianteId = 1,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var prestamo = new Prestamo
        {
            LibroId = 1,
            EstudianteId = 1,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = DateTime.Now.AddDays(7)
        };

        var libro = new Libro { LibroId = 1, Estado = EstadoLibro.Disponible };

        _mockUnitOfWork.Setup(uow => uow.Libros.GetByIdAsync(prestamo.LibroId))
            .ReturnsAsync(libro);
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId))
            .ReturnsAsync(new List<Prestamo>());
        _mockUnitOfWork.Setup(uow => uow.Estudiantes.GetByIdAsync(prestamo.EstudianteId))
            .ReturnsAsync(new Estudiante { EstudianteId = prestamo.EstudianteId });
        _mockUnitOfWork.Setup(uow => uow.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId))
            .ReturnsAsync(new List<Prestamo>());

        _mockMapper.Setup(m => m.Map<Prestamo>(prestamoCreateDto))
            .Returns(prestamo);

        // Act
        var result = await _service.CreateAsync(prestamoCreateDto);

        // Assert
        Assert.True(result);
        Assert.Equal(EstadoPrestamo.Activo, prestamo.Estado);
        Assert.Equal(EstadoLibro.Prestado, libro.Estado);
        _mockUnitOfWork.Verify(uow => uow.Libros.Update(libro), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}