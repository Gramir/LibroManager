using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using LibroManager.Services.Interfaces;
using AutoMapper;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class LibroServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILibroValidationService> _mockValidationService;
    private readonly Mock<ILibroRepository> _mockLibroRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly LibroService _libroService;

    public LibroServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockValidationService = new Mock<ILibroValidationService>();
        _mockLibroRepository = new Mock<ILibroRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUnitOfWork.Setup(u => u.Libros).Returns(_mockLibroRepository.Object);
        
        _libroService = new LibroService(_mockUnitOfWork.Object, _mockValidationService.Object, _mockMapper.Object);
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
            CategoriaId = 1
        };

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
        _mockMapper.Setup(m => m.Map<Libro>(libroCreateDto))
            .Returns(libro);

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
            Categoria = new Categoria { Nombre = "Test Categoria" }
        };

        var libroDto = new LibroDTO
        {
            LibroId = 1,
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorNombre = "Test Autor",
            CategoriaNombre = "Test Categoria"
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
            AutorId = 1,
            CategoriaId = 1
        };

        var libro = new Libro 
        { 
            LibroId = 1,
            Titulo = "Test Libro",
            ISBN = "1234567890",
            AutorId = 1,
            CategoriaId = 1
        };

        _mockValidationService.Setup(s => s.LibroEsValido(It.IsAny<Libro>()))
            .ReturnsAsync(true);
        _mockLibroRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(libro);
        _mockLibroRepository.Setup(r => r.IsbnExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<Libro>(libroUpdateDto))
            .Returns(libro);

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
                Categoria = new Categoria { Nombre = "Categoria 1" }
            }
        };

        var librosDto = new List<LibroDTO>
        {
            new() 
            { 
                LibroId = 1, 
                Titulo = "Libro 1",
                AutorNombre = "Autor 1",
                CategoriaNombre = "Categoria 1"
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
}