using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class CategoriaServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CategoriaService _service;

    public CategoriaServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _service = new CategoriaService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategorias()
    {
        // Arrange
        var expectedCategorias = new List<Categoria>
        {
            new() { CategoriaId = 1, Nombre = "Categoria 1" },
            new() { CategoriaId = 2, Nombre = "Categoria 2" }
        };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetAllAsync())
            .ReturnsAsync(expectedCategorias);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(expectedCategorias.Count, result.Count());
    }

    [Fact]
    public async Task GetByNombreAsync_ReturnsCategoria_WhenNombreExists()
    {
        // Arrange
        var nombre = "Test Categoria";
        var expectedCategoria = new Categoria { CategoriaId = 1, Nombre = nombre };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(nombre))
            .ReturnsAsync(expectedCategoria);

        // Act
        var result = await _service.GetByNombreAsync(nombre);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(nombre, result.Nombre);
    }

    [Fact]
    public async Task GetCategoriasWithLibrosAsync_ReturnsCategoriasWithLibros()
    {
        // Arrange
        var expectedCategorias = new List<Categoria>
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

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetCategoriasWithLibrosAsync())
            .ReturnsAsync(expectedCategorias);

        // Act
        var result = await _service.GetCategoriasWithLibrosAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result.First().Libros?.Count);
    }

    [Fact]
    public async Task CreateAsync_ReturnsTrue_WhenNombreDoesNotExist()
    {
        // Arrange
        var categoria = new Categoria { Nombre = "Nueva Categoria" };
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(categoria.Nombre))
            .ReturnsAsync((Categoria?)null);
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoria.CategoriaId))
            .ReturnsAsync(categoria);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(categoria);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.AddAsync(categoria), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenNombreExists()
    {
        // Arrange
        var categoria = new Categoria { Nombre = "Categoria Existente" };
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(categoria.Nombre))
            .ReturnsAsync(new Categoria { CategoriaId = 1, Nombre = categoria.Nombre });

        // Act
        var result = await _service.CreateAsync(categoria);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.AddAsync(categoria), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsTrue_WhenNombreDoesNotExist()
    {
        // Arrange
        var categoria = new Categoria { CategoriaId = 1, Nombre = "Categoria Actualizada" };
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(categoria.Nombre))
            .ReturnsAsync((Categoria?)null);
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoria.CategoriaId))
            .ReturnsAsync(categoria);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(categoria);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Update(categoria), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenNombreExistsForDifferentCategoria()
    {
        // Arrange
        var categoria = new Categoria { CategoriaId = 1, Nombre = "Categoria Existente" };
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(categoria.Nombre))
            .ReturnsAsync(new Categoria { CategoriaId = 2, Nombre = categoria.Nombre });

        // Act
        var result = await _service.UpdateAsync(categoria);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Update(categoria), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenCategoriaExistsWithoutLibros()
    {
        // Arrange
        var categoriaId = 1;
        var categoria = new Categoria { CategoriaId = categoriaId, Nombre = "Categoria a Eliminar" };
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoriaId))
            .ReturnsAsync(categoria);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteAsync(categoriaId);

        // Assert
        Assert.True(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Remove(categoria), Times.Once);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenCategoriaHasLibros()
    {
        // Arrange
        var categoriaId = 1;
        var categoria = new Categoria 
        { 
            CategoriaId = categoriaId, 
            Nombre = "Categoria con Libros",
            Libros = new List<Libro> { new() { LibroId = 1, Titulo = "Libro 1" } }
        };
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoriaId))
            .ReturnsAsync(categoria);

        // Act
        var result = await _service.DeleteAsync(categoriaId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Remove(It.IsAny<Categoria>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenCategoriaDoesNotExist()
    {
        // Arrange
        var categoriaId = 1;
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoriaId))
            .ReturnsAsync((Categoria?)null);

        // Act
        var result = await _service.DeleteAsync(categoriaId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Remove(It.IsAny<Categoria>()), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenNombreIsEmpty()
    {
        // Arrange
        var categoria = new Categoria { Nombre = "" };

        // Act
        var result = await _service.CreateAsync(categoria);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.AddAsync(categoria), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenNombreExceedsMaxLength()
    {
        // Arrange
        var categoria = new Categoria { 
            Nombre = new string('A', 51) // Excede el límite de 50 caracteres
        };

        // Act
        var result = await _service.CreateAsync(categoria);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.AddAsync(categoria), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_ReturnsFalse_WhenDatabaseError()
    {
        // Arrange
        var categoria = new Categoria { Nombre = "Test Categoria" };
        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByNombreAsync(categoria.Nombre))
            .ReturnsAsync((Categoria?)null);
        _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync())
            .ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _service.CreateAsync(categoria);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenCategoriaNoExiste()
    {
        // Arrange
        var categoria = new Categoria { 
            CategoriaId = 999, 
            Nombre = "Categoria Inexistente" 
        };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoria.CategoriaId))
            .ReturnsAsync((Categoria?)null);

        // Act
        var result = await _service.UpdateAsync(categoria);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Update(categoria), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenNombreIsEmpty()
    {
        // Arrange
        var categoria = new Categoria { 
            CategoriaId = 1, 
            Nombre = "" 
        };

        // Act
        var result = await _service.UpdateAsync(categoria);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Update(categoria), Times.Never);
        _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenCategoriaHasActiveBooks()
    {
        // Arrange
        var categoriaId = 1;
        var categoria = new Categoria { 
            CategoriaId = categoriaId,
            Nombre = "Test Categoria",
            Libros = new List<Libro> {
                new() { LibroId = 1, Titulo = "Libro Activo" }
            }
        };

        _mockUnitOfWork.Setup(uow => uow.Categorias.GetByIdAsync(categoriaId))
            .ReturnsAsync(categoria);

        // Act
        var result = await _service.DeleteAsync(categoriaId);

        // Assert
        Assert.False(result);
        _mockUnitOfWork.Verify(uow => uow.Categorias.Remove(categoria), Times.Never);
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
        var result = await _service.GetByNombreAsync(nombre);

        // Assert
        Assert.Null(result);
    }
}