using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using AutoMapper;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class CategoriaServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICategoriaRepository> _mockCategoriaRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoriaService _categoriaService;

    public CategoriaServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCategoriaRepository = new Mock<ICategoriaRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUnitOfWork.Setup(u => u.Categorias).Returns(_mockCategoriaRepository.Object);
        _categoriaService = new CategoriaService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategorias()
    {
        // Arrange
        var categorias = new List<Categoria>
        {
            new() { CategoriaId = 1, Nombre = "Categoria 1" },
            new() { CategoriaId = 2, Nombre = "Categoria 2" }
        };

        var categoriasDto = new List<CategoriaDTO>
        {
            new() { CategoriaId = 1, Nombre = "Categoria 1" },
            new() { CategoriaId = 2, Nombre = "Categoria 2" }
        };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetAllAsync())
            .ReturnsAsync(categorias);
        _mockMapper.Setup(m => m.Map<IEnumerable<CategoriaDTO>>(categorias))
            .Returns(categoriasDto);

        // Act
        var result = await _categoriaService.GetAllAsync();

        // Assert
        Assert.Equal(categoriasDto.Count, result.Count());
    }

    [Fact]
    public async Task GetByNombreAsync_ReturnsCategoria_WhenNombreExists()
    {
        // Arrange
        var nombre = "Test Categoria";
        var categoria = new Categoria { CategoriaId = 1, Nombre = nombre };
        var categoriaDto = new CategoriaDTO { CategoriaId = 1, Nombre = nombre };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(nombre))
            .ReturnsAsync(categoria);
        _mockMapper.Setup(m => m.Map<CategoriaDTO>(categoria))
            .Returns(categoriaDto);

        // Act
        var result = await _categoriaService.GetByNombreAsync(nombre);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nombre, result.Nombre);
    }

    [Fact]
    public async Task GetCategoriasWithLibrosAsync_ReturnsCategoriasWithLibros()
    {
        // Arrange
        var categorias = new List<Categoria>
        {
            new() 
            { 
                CategoriaId = 1, 
                Nombre = "Categoria 1",
                Libros = new List<Libro> 
                { 
                    new() { LibroId = 1, Titulo = "Libro 1" },
                    new() { LibroId = 2, Titulo = "Libro 2" }
                }
            }
        };

        var categoriasDto = new List<CategoriaDTO>
        {
            new() 
            { 
                CategoriaId = 1, 
                Nombre = "Categoria 1",
                CantidadLibros = 2
            }
        };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetCategoriasWithLibrosAsync())
            .ReturnsAsync(categorias);
        _mockMapper.Setup(m => m.Map<IEnumerable<CategoriaDTO>>(categorias))
            .Returns(categoriasDto);

        // Act
        var result = await _categoriaService.GetCategoriasWithLibrosAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result.First().CantidadLibros);
    }

    [Fact]
    public async Task CreateAsync_ReturnsTrue_WhenNombreDoesNotExist()
    {
        // Arrange
        var categoriaCreateDto = new CategoriaCreateDTO { Nombre = "Nueva Categoria" };
        var categoria = new Categoria { Nombre = "Nueva Categoria" };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(categoria.Nombre))
            .ReturnsAsync((Categoria?)null);
        _mockMapper.Setup(m => m.Map<Categoria>(categoriaCreateDto))
            .Returns(categoria);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoriaService.CreateAsync(categoriaCreateDto);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.AddAsync(categoria), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenNombreExists()
    {
        // Arrange
        var categoriaCreateDto = new CategoriaCreateDTO { Nombre = "Categoria Existente" };
        var categoria = new Categoria { Nombre = "Categoria Existente" };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(categoria.Nombre))
            .ReturnsAsync(new Categoria { CategoriaId = 1, Nombre = categoria.Nombre });
        _mockMapper.Setup(m => m.Map<Categoria>(categoriaCreateDto))
            .Returns(categoria);

        // Act
        var result = await _categoriaService.CreateAsync(categoriaCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.AddAsync(categoria), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenNombreDoesNotExist()
    {
        // Arrange
        var categoriaUpdateDto = new CategoriaUpdateDTO { CategoriaId = 1, Nombre = "Categoria Actualizada" };
        var categoria = new Categoria { CategoriaId = 1, Nombre = "Categoria Actualizada" };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoria.CategoriaId))
            .ReturnsAsync(categoria);
        _mockMapper.Setup(m => m.Map<Categoria>(categoriaUpdateDto))
            .Returns(categoria);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoriaService.UpdateAsync(categoriaUpdateDto);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Update(categoria), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenCategoriaNoExiste()
    {
        // Arrange
        var categoriaUpdateDto = new CategoriaUpdateDTO 
        { 
            CategoriaId = 999, 
            Nombre = "Categoria Inexistente" 
        };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoriaUpdateDto.CategoriaId))
            .ReturnsAsync((Categoria?)null);

        // Act
        var result = await _categoriaService.UpdateAsync(categoriaUpdateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Update(It.IsAny<Categoria>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenCategoriaExists()
    {
        // Arrange
        var categoriaId = 1;
        var categoria = new Categoria { CategoriaId = categoriaId, Nombre = "Categoria a Eliminar" };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoriaId))
            .ReturnsAsync(categoria);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _categoriaService.DeleteAsync(categoriaId);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Remove(categoria), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenCategoriaDoesNotExist()
    {
        // Arrange
        var categoriaId = 1;
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoriaId))
            .ReturnsAsync((Categoria?)null);

        // Act
        var result = await _categoriaService.DeleteAsync(categoriaId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Remove(It.IsAny<Categoria>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenNombreIsEmpty()
    {
        // Arrange
        var categoriaCreateDto = new CategoriaCreateDTO { Nombre = "" };

        // Act
        var result = await _categoriaService.CreateAsync(categoriaCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.AddAsync(It.IsAny<Categoria>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenNombreExceedsMaxLength()
    {
        // Arrange
        var categoriaCreateDto = new CategoriaCreateDTO 
        { 
            Nombre = new string('A', 51) // Excede el límite de 50 caracteres
        };

        // Act
        var result = await _categoriaService.CreateAsync(categoriaCreateDto);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.AddAsync(It.IsAny<Categoria>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetByNombreAsync_ReturnsNull_WhenDatabaseError()
    {
        // Arrange
        var nombre = "Test Categoria";
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(nombre))
            .ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _categoriaService.GetByNombreAsync(nombre);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenExceptionOccurs()
    {
        // Arrange
        _mockCategoriaRepository.Setup(r => r.GetAllAsync())
            .ThrowsAsync(new Exception());

        // Act
        var result = await _categoriaService.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenExceptionOccurs()
    {
        // Arrange
        _mockCategoriaRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _categoriaService.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCategoriasWithLibrosAsync_ReturnsEmptyList_WhenExceptionOccurs()
    {
        // Arrange
        _mockCategoriaRepository.Setup(r => r.GetCategoriasWithLibrosAsync())
            .ThrowsAsync(new Exception());

        // Act
        var result = await _categoriaService.GetCategoriasWithLibrosAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenNombreIsNull()
    {
        // Arrange
        var categoriaDto = new CategoriaCreateDTO { Nombre = string.Empty };

        // Act
        var result = await _categoriaService.CreateAsync(categoriaDto);

        // Assert
        Assert.False(result);
        _mockCategoriaRepository.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenCategoriaAlreadyExists()
    {
        // Arrange
        var categoriaDto = new CategoriaCreateDTO { Nombre = "Test" };
        var existingCategoria = new Categoria { CategoriaId = 1, Nombre = "Test" };

        _mockCategoriaRepository.Setup(r => r.GetByNombreAsync("Test"))
            .ReturnsAsync(existingCategoria);

        // Act
        var result = await _categoriaService.CreateAsync(categoriaDto);

        // Assert
        Assert.False(result);
        _mockCategoriaRepository.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var categoriaDto = new CategoriaCreateDTO { Nombre = "Test" };
        _mockCategoriaRepository.Setup(r => r.AddAsync(It.IsAny<Categoria>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _categoriaService.CreateAsync(categoriaDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenNombreIsInvalid()
    {
        // Arrange
        var categoriaDto = new CategoriaUpdateDTO 
        { 
            CategoriaId = 1,
            Nombre = new string('A', 51)
        };

        // Act
        var result = await _categoriaService.UpdateAsync(categoriaDto);

        // Assert
        Assert.False(result);
        _mockCategoriaRepository.Verify(r => r.Update(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenCategoriaNotFound()
    {
        // Arrange
        var categoriaDto = new CategoriaUpdateDTO 
        { 
            CategoriaId = 1,
            Nombre = "Test"
        };

        _mockCategoriaRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(() => null!);

        // Act
        var result = await _categoriaService.UpdateAsync(categoriaDto);

        // Assert
        Assert.False(result);
        _mockCategoriaRepository.Verify(r => r.Update(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var categoriaDto = new CategoriaUpdateDTO 
        { 
            CategoriaId = 1,
            Nombre = "Test"
        };

        var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };

        _mockCategoriaRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(categoria);
        _mockCategoriaRepository.Setup(r => r.Update(It.IsAny<Categoria>()))
            .Throws(new Exception());

        // Act
        var result = await _categoriaService.UpdateAsync(categoriaDto);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenCategoriaNotFound()
    {
        // Arrange
        _mockCategoriaRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(() => null!);

        // Act
        var result = await _categoriaService.DeleteAsync(1);

        // Assert
        Assert.False(result);
        _mockCategoriaRepository.Verify(r => r.Remove(It.IsAny<Categoria>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenExceptionOccurs()
    {
        // Arrange
        var categoria = new Categoria { CategoriaId = 1, Nombre = "Test" };

        _mockCategoriaRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(categoria);
        _mockCategoriaRepository.Setup(r => r.Remove(It.IsAny<Categoria>()))
            .Throws(new Exception());

        // Act
        var result = await _categoriaService.DeleteAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCategoriaDTOs_WhenSuccessful()
    {
        // Arrange
        var categorias = new List<Categoria>
        {
            new() { CategoriaId = 1, Nombre = "Test1" },
            new() { CategoriaId = 2, Nombre = "Test2" }
        };

        var categoriasDto = new List<CategoriaDTO>
        {
            new() { CategoriaId = 1, Nombre = "Test1" },
            new() { CategoriaId = 2, Nombre = "Test2" }
        };

        _mockCategoriaRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(categorias);
        _mockMapper.Setup(m => m.Map<IEnumerable<CategoriaDTO>>(categorias))
            .Returns(categoriasDto);

        // Act
        var result = await _categoriaService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }
}