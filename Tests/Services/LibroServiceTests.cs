using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using LibroManager.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class LibroServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILibroValidationService> _mockValidationService;
    private readonly Mock<ILibroRepository> _mockLibroRepository;
    private readonly Mock<IAutorRepository> _mockAutorRepository;
    private readonly Mock<ICategoriaRepository> _mockCategoriaRepository;
    private readonly Mock<IUbicacionRepository> _mockUbicacionRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<LibroService>> _mockLogger;
    private readonly LibroService _libroService;

    public LibroServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockValidationService = new Mock<ILibroValidationService>();
        _mockLibroRepository = new Mock<ILibroRepository>();
        _mockAutorRepository = new Mock<IAutorRepository>();
        _mockCategoriaRepository = new Mock<ICategoriaRepository>();
        _mockUbicacionRepository = new Mock<IUbicacionRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<LibroService>>();

        _mockUnitOfWork.Setup(u => u.Libros).Returns(_mockLibroRepository.Object);
        _mockUnitOfWork.Setup(u => u.Autores).Returns(_mockAutorRepository.Object);
        _mockUnitOfWork.Setup(u => u.Categorias).Returns(_mockCategoriaRepository.Object);
        _mockUnitOfWork.Setup(u => u.Ubicaciones).Returns(_mockUbicacionRepository.Object);
        
        _libroService = new LibroService(_mockUnitOfWork.Object, _mockValidationService.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateLibroAsync_WithValidLibro_ReturnsTrue()
    {
        // Arrange
        var libroCreateDto = new LibroCreateDTO 
        { 
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1,
            UbicacionString = "A-1-1" // Este campo es correcto para LibroCreateDTO
        };

        var libro = new Libro 
        { 
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1,
            UbicacionId = 1
        };

        var ubicaciones = new List<Ubicacion>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 }
        };

        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(true);
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockLibroRepository.Setup(r => r.SerialExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<Libro>(libroCreateDto))
            .Returns(libro);
        _mockUbicacionRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(ubicaciones);

        // Act
        var result = await _libroService.CreateLibroAsync(libroCreateDto);

        // Assert
        Assert.True(result);
        _mockLibroRepository.Verify(r => r.AddAsync(It.IsAny<Libro>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetLibroByIdAsync_ReturnsLibroDTO()
    {
        // Arrange
        var libro = new Libro 
        { 
            LibroId = 1,
            Titulo = "Test Libro",
            ISBN = "1234567890",
            Autor = new Autor { Nombre = "Test Autor" },
            Categoria = new Categoria { Nombre = "Test Categoria" },
            UbicacionId = 1,
            Ubicacion = new Ubicacion { Estante = "A", Nivel = 1, Posicion = 1 }
        };

        var libroDto = new LibroDTO
        {
            LibroId = 1,
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorNombre = "Test Autor",
            CategoriaNombre = "Test Categoria",
            UbicacionFormateada = "A-1-1"
        };

        _mockLibroRepository.Setup(r => r.GetLibroWithDetailsAsync(1))
            .ReturnsAsync(libro);
        _mockMapper.Setup(m => m.Map<LibroDTO>(libro))
            .Returns(libroDto);

        // Act
        var result = await _libroService.GetLibroByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(libroDto.Titulo, result.Titulo);
        Assert.Equal(libroDto.AutorNombre, result.AutorNombre);
        Assert.Equal(libroDto.CategoriaNombre, result.CategoriaNombre);
    }

    [Fact]
    public async Task UpdateLibroAsync_WithValidLibroAndNewIsbn_ReturnsTrue()
    {
        // Arrange
        var libroUpdateDto = new LibroUpdateDTO 
        { 
            LibroId = 1,
            Titulo = "Test Libro",
            ISBN = "1234567890",
            Serial = "TST-LBR-001",
            AutorId = 1,
            CategoriaId = 1,
            UbicacionString = "A-1-1"
        };

        var libro = new Libro 
        { 
            LibroId = 1,
            Titulo = "Test Libro",
            ISBN = "0987654321",
            Serial = "TST-LBR-002",
            AutorId = 1,
            CategoriaId = 1,
            UbicacionId = 1
        };

        var ubicaciones = new List<Ubicacion>
        {
            new() { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 }
        };

        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(true);
        _mockLibroRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(libro);
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync(libroUpdateDto.ISBN))
            .ReturnsAsync(false);
        _mockLibroRepository.Setup(r => r.SerialExistsAsync(libroUpdateDto.Serial))
            .ReturnsAsync(false);
        _mockUbicacionRepository.Setup(u => u.GetAllAsync())
            .ReturnsAsync(ubicaciones);

        // Act
        var result = await _libroService.UpdateLibroAsync(libroUpdateDto);

        // Assert
        Assert.True(result);
        _mockLibroRepository.Verify(r => r.Update(It.IsAny<Libro>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
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
    public async Task GetAllLibrosAsync_ReturnsAllLibrosDTO()
    {
        // Arrange
        var libros = new List<Libro>
        {
            new() 
            { 
                LibroId = 1, 
                Titulo = "Libro 1",
                Autor = new Autor { Nombre = "Autor 1" },
                Categoria = new Categoria { Nombre = "Categoria 1" },
                UbicacionId = 1,
                Ubicacion = new Ubicacion { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 }
            }
        };

        var librosDto = new List<LibroDTO>
        {
            new() 
            { 
                LibroId = 1, 
                Titulo = "Libro 1",
                AutorNombre = "Autor 1",
                CategoriaNombre = "Categoria 1",
                UbicacionFormateada = "A-1-1"
            }
        };

        _mockLibroRepository.Setup(r => r.GetLibrosWithAutorAndCategoriaAsync())
            .ReturnsAsync(libros);
        _mockMapper.Setup(m => m.Map<IEnumerable<LibroDTO>>(libros))
            .Returns(librosDto);

        // Act
        var result = await _libroService.GetAllLibrosAsync();

        // Assert
        Assert.Single(result);
        var libro = result.First();
        Assert.Equal("Libro 1", libro.Titulo);
        Assert.Equal("Autor 1", libro.AutorNombre);
        Assert.Equal("Categoria 1", libro.CategoriaNombre);
    }

    [Fact]
    public async Task CreateLibroAsync_WithInvalidLibro_ReturnsFalse()
    {
        // Arrange
        var libroCreateDto = new LibroCreateDTO 
        { 
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1,
            UbicacionString = "A-1-1"
        };

        var libro = new Libro 
        { 
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1
        };

        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<Libro>(libroCreateDto))
            .Returns(libro);

        // Act
        var result = await _libroService.CreateLibroAsync(libroCreateDto);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.AddAsync(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetLibroByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        _mockLibroRepository.Setup(r => r.GetLibroWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((Libro?)null);

        // Act
        var result = await _libroService.GetLibroByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateLibroAsync_WithInvalidLibro_ReturnsFalse()
    {
        // Arrange
        var libroUpdateDto = new LibroUpdateDTO 
        { 
            LibroId = 1,
            Titulo = "",  // Invalid title
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1,
            UbicacionString = "A-1-1"
        };

        var libro = new Libro();
        _mockLibroRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(libro);
        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(false);

        // Act
        var result = await _libroService.UpdateLibroAsync(libroUpdateDto);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.Update(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateLibroAsync_WithNonExistentLibro_ReturnsFalse()
    {
        // Arrange
        var libroUpdateDto = new LibroUpdateDTO 
        { 
            LibroId = 999,
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1,
            UbicacionString = "A-1-1"
        };

        _mockLibroRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Libro?)null);

        // Act
        var result = await _libroService.UpdateLibroAsync(libroUpdateDto);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.Update(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteLibroAsync_WithNonExistentLibro_ReturnsFalse()
    {
        // Arrange
        _mockLibroRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Libro?)null);

        // Act
        var result = await _libroService.DeleteLibroAsync(999);

        // Assert
        Assert.False(result);
        _mockLibroRepository.Verify(r => r.Remove(It.IsAny<Libro>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ExisteIsbnAsync_WithExistingIsbn_ReturnsTrue()
    {
        // Arrange
        string isbn = "1234567890";
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync(isbn))
            .ReturnsAsync(true);

        // Act
        var result = await _libroService.ExisteIsbnAsync(isbn);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExisteIsbnAsync_WithNonExistingIsbn_ReturnsFalse()
    {
        // Arrange
        string isbn = "0987654321";
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync(isbn))
            .ReturnsAsync(false);

        // Act
        var result = await _libroService.ExisteIsbnAsync(isbn);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetLibrosByAutorIdAsync_WithValidAutorAndLibros_ReturnsLibrosDTO()
    {
        // Arrange
        var autor = new Autor 
        { 
            AutorId = 1, 
            Nombre = "Test Autor",
            Libros = new List<Libro>
            {
                new() 
                { 
                    LibroId = 1,
                    Titulo = "Libro 1",
                    ISBN = "1234567890",
                    Categoria = new Categoria { Nombre = "Categoría 1" },
                    UbicacionId = 1,
                    Ubicacion = new Ubicacion { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 }
                },
                new() 
                { 
                    LibroId = 2,
                    Titulo = "Libro 2",
                    ISBN = "0987654321",
                    Categoria = new Categoria { Nombre = "Categoría 2" },
                    UbicacionId = 2,
                    Ubicacion = new Ubicacion { UbicacionId = 2, Estante = "B", Nivel = 2, Posicion = 2 }
                }
            }
        };

        var librosDto = new List<LibroDTO>
        {
            new() 
            { 
                LibroId = 1,
                Titulo = "Libro 1",
                ISBN = "1234567890",
                CategoriaNombre = "Categoría 1",
                UbicacionFormateada = "A-1-1"
            },
            new() 
            { 
                LibroId = 2,
                Titulo = "Libro 2",
                ISBN = "0987654321",
                CategoriaNombre = "Categoría 2",
                UbicacionFormateada = "B-2-2"
            }
        };

        _mockAutorRepository.Setup(r => r.GetAutorWithLibrosAsync(1))
            .ReturnsAsync(autor);
        _mockMapper.Setup(m => m.Map<IEnumerable<LibroDTO>>(autor.Libros))
            .Returns(librosDto);

        // Act
        var result = await _libroService.GetLibrosByAutorIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Collection(result,
            libro => Assert.Equal("Libro 1", libro.Titulo),
            libro => Assert.Equal("Libro 2", libro.Titulo)
        );
    }

    [Fact]
    public async Task GetLibrosByAutorIdAsync_WithAutorWithoutLibros_ReturnsEmptyList()
    {
        // Arrange
        var autor = new Autor 
        { 
            AutorId = 1, 
            Nombre = "Test Autor",
            Libros = new List<Libro>()
        };

        _mockAutorRepository.Setup(r => r.GetAutorWithLibrosAsync(1))
            .ReturnsAsync(autor);
        _mockMapper.Setup(m => m.Map<IEnumerable<LibroDTO>>(autor.Libros))
            .Returns(new List<LibroDTO>());

        // Act
        var result = await _libroService.GetLibrosByAutorIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLibrosByAutorIdAsync_WithNonExistentAutor_ReturnsEmptyList()
    {
        // Arrange
        _mockAutorRepository.Setup(r => r.GetAutorWithLibrosAsync(999))
            .ReturnsAsync((Autor?)null);

        // Act
        var result = await _libroService.GetLibrosByAutorIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLibrosByAutorIdAsync_WithLibrosWithoutCategoria_LoadsCategorias()
    {
        // Arrange
        var categoria1 = new Categoria { CategoriaId = 1, Nombre = "Categoría 1" };
        var categoria2 = new Categoria { CategoriaId = 2, Nombre = "Categoría 2" };
        var ubicacion1 = new Ubicacion { UbicacionId = 1, Estante = "A", Nivel = 1, Posicion = 1 };
        var ubicacion2 = new Ubicacion { UbicacionId = 2, Estante = "B", Nivel = 2, Posicion = 2 };

        var autor = new Autor 
        { 
            AutorId = 1, 
            Nombre = "Test Autor",
            Libros = new List<Libro>
            {
                new() 
                { 
                    LibroId = 1,
                    Titulo = "Libro 1",
                    ISBN = "1234567890",
                    CategoriaId = 1,
                    UbicacionId = 1
                },
                new() 
                { 
                    LibroId = 2,
                    Titulo = "Libro 2",
                    ISBN = "0987654321",
                    CategoriaId = 2,
                    UbicacionId = 2
                }
            }
        };

        var librosDto = new List<LibroDTO>
        {
            new() 
            { 
                LibroId = 1,
                Titulo = "Libro 1",
                ISBN = "1234567890",
                CategoriaNombre = "Categoría 1",
                UbicacionFormateada = "A-1-1"
            },
            new() 
            { 
                LibroId = 2,
                Titulo = "Libro 2",
                ISBN = "0987654321",
                CategoriaNombre = "Categoría 2",
                UbicacionFormateada = "B-2-2"
            }
        };

        _mockAutorRepository.Setup(r => r.GetAutorWithLibrosAsync(1))
            .ReturnsAsync(autor);
        _mockCategoriaRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(categoria1);
        _mockCategoriaRepository.Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(categoria2);
        _mockUbicacionRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(ubicacion1);
        _mockUbicacionRepository.Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(ubicacion2);
        _mockMapper.Setup(m => m.Map<IEnumerable<LibroDTO>>(It.IsAny<IEnumerable<Libro>>()))
            .Returns(librosDto);

        // Act
        var result = await _libroService.GetLibrosByAutorIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockCategoriaRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
        _mockUbicacionRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ContarEjemplaresPrestadosPorIsbnAsync_ConLibrosPrestados_ReturnsCantidadCorrecta()
    {
        // Arrange
        var isbn = "1234567890";
        var libros = new List<Libro>
        {
            new() { ISBN = isbn, Estado = EstadoLibro.Prestado },
            new() { ISBN = isbn, Estado = EstadoLibro.Prestado },
            new() { ISBN = isbn, Estado = EstadoLibro.Disponible },
            new() { ISBN = "9876543210", Estado = EstadoLibro.Prestado } // Otro ISBN
        };

        _mockUnitOfWork.Setup(uow => uow.Libros.GetAllAsync())
            .ReturnsAsync(libros);

        // Act
        var result = await _libroService.ContarEjemplaresPrestadosPorIsbnAsync(isbn);

        // Assert
        Assert.Equal(2, result);
        _mockUnitOfWork.Verify(uow => uow.Libros.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ContarEjemplaresPrestadosPorIsbnAsync_SinLibrosPrestados_ReturnsZero()
    {
        // Arrange
        var isbn = "1234567890";
        var libros = new List<Libro>
        {
            new() { ISBN = isbn, Estado = EstadoLibro.Disponible },
            new() { ISBN = isbn, Estado = EstadoLibro.Disponible },
            new() { ISBN = "9876543210", Estado = EstadoLibro.Prestado } // Otro ISBN
        };

        _mockUnitOfWork.Setup(uow => uow.Libros.GetAllAsync())
            .ReturnsAsync(libros);

        // Act
        var result = await _libroService.ContarEjemplaresPrestadosPorIsbnAsync(isbn);

        // Assert
        Assert.Equal(0, result);
        _mockUnitOfWork.Verify(uow => uow.Libros.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ContarEjemplaresPrestadosPorIsbnAsync_CuandoNoExistenLibros_ReturnsZero()
    {
        // Arrange
        var isbn = "1234567890";
        var libros = new List<Libro>();

        _mockUnitOfWork.Setup(uow => uow.Libros.GetAllAsync())
            .ReturnsAsync(libros);

        // Act
        var result = await _libroService.ContarEjemplaresPrestadosPorIsbnAsync(isbn);

        // Assert
        Assert.Equal(0, result);
        _mockUnitOfWork.Verify(uow => uow.Libros.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ContarEjemplaresPrestadosPorIsbnAsync_CuandoOcurreError_ReturnsZero()
    {
        // Arrange
        var isbn = "1234567890";
        _mockUnitOfWork.Setup(uow => uow.Libros.GetAllAsync())
            .ThrowsAsync(new Exception("Error simulado"));

        // Act
        var result = await _libroService.ContarEjemplaresPrestadosPorIsbnAsync(isbn);

        // Assert
        Assert.Equal(0, result);
        _mockUnitOfWork.Verify(uow => uow.Libros.GetAllAsync(), Times.Once);
        _mockLogger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error al contar ejemplares prestados")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()!),
            Times.Once);
    }
}