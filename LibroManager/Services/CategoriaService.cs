using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class CategoriaService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoriaService> logger) : ICategoriaService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CategoriaService> _logger = logger;
    private const int MAX_NOMBRE_LENGTH = 50;

    public async Task<IEnumerable<CategoriaDTO>> GetAllAsync()
    {
        try
        {
            var categorias = await _unitOfWork.Categorias.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las categorías");
            return [];
        }
    }

    public async Task<CategoriaDTO?> GetByIdAsync(int id)
    {
        try
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);
            return _mapper.Map<CategoriaDTO>(categoria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categoría con ID {CategoriaId}", id);
            return null;
        }
    }

    public async Task<CategoriaDTO?> GetByNombreAsync(string nombre)
    {
        try
        {
            var categoria = await _unitOfWork.Categorias.GetByNombreAsync(nombre);
            return _mapper.Map<CategoriaDTO>(categoria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categoría por nombre: {Nombre}", nombre);
            return null;
        }
    }

    public async Task<IEnumerable<CategoriaDTO>> GetCategoriasWithLibrosAsync()
    {
        try
        {
            var categorias = await _unitOfWork.Categorias.GetCategoriasWithLibrosAsync();
            return _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías con libros");
            return [];
        }
    }

    public async Task<bool> CreateAsync(CategoriaCreateDTO categoriaDto)
    {
        try
        {
            if (!ValidateCategoriaData(categoriaDto))
            {
                _logger.LogWarning("Datos de categoría no válidos durante la creación: {Nombre}", categoriaDto.Nombre);
                return false;
            }

            if (await _unitOfWork.Categorias.GetByNombreAsync(categoriaDto.Nombre) != null)
            {
                _logger.LogWarning("Ya existe una categoría con el nombre: {Nombre}", categoriaDto.Nombre);
                return false;
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Categoría creada: {CategoriaId}, Nombre: {Nombre}",
                categoria.CategoriaId, categoria.Nombre);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear categoría: {Nombre}", categoriaDto.Nombre);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(CategoriaUpdateDTO categoriaDto)
    {
        try
        {
            if (!ValidateCategoriaData(categoriaDto))
            {
                _logger.LogWarning("Datos de categoría no válidos durante la actualización: {CategoriaId}", categoriaDto.CategoriaId);
                return false;
            }

            var existingCategoria = await _unitOfWork.Categorias.GetByIdAsync(categoriaDto.CategoriaId);
            if (existingCategoria == null)
            {
                _logger.LogWarning("Categoría no encontrada para actualización: {CategoriaId}", categoriaDto.CategoriaId);
                return false;
            }

            // Verificar si el nombre ya existe en otra categoría
            if (existingCategoria.Nombre != categoriaDto.Nombre)
            {
                var existingByName = await _unitOfWork.Categorias.GetByNombreAsync(categoriaDto.Nombre);
                if (existingByName != null && existingByName.CategoriaId != categoriaDto.CategoriaId)
                {
                    _logger.LogWarning("Ya existe otra categoría con el nombre: {Nombre}", categoriaDto.Nombre);
                    return false;
                }
            }

            existingCategoria.Nombre = categoriaDto.Nombre;

            _unitOfWork.Categorias.Update(existingCategoria);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Categoría actualizada: {CategoriaId}, Nombre: {Nombre}",
                existingCategoria.CategoriaId, existingCategoria.Nombre);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar categoría {CategoriaId}", categoriaDto.CategoriaId);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);
            if (categoria == null)
            {
                _logger.LogWarning("Categoría no encontrada para eliminación: {CategoriaId}", id);
                return false;
            }

            // Verificar si hay libros asociados a esta categoría
            var libros = await _unitOfWork.Libros.GetAllAsync();
            var hasLibros = libros.Any(l => l.CategoriaId == id);
            if (hasLibros)
            {
                _logger.LogWarning("No se puede eliminar categoría {CategoriaId} porque tiene libros asociados", id);
                return false;
            }

            _unitOfWork.Categorias.Remove(categoria);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Categoría eliminada: {CategoriaId}, Nombre: {Nombre}",
                categoria.CategoriaId, categoria.Nombre);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar categoría {CategoriaId}", id);
            return false;
        }
    }

    private bool ValidateCategoriaData(CategoriaCreateDTO categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Nombre))
        {
            _logger.LogWarning("Nombre de categoría vacío");
            return false;
        }

        if (categoria.Nombre.Length > MAX_NOMBRE_LENGTH)
        {
            _logger.LogWarning("Nombre de categoría excede el límite de {MaxLength} caracteres: {Length}",
                MAX_NOMBRE_LENGTH, categoria.Nombre.Length);
            return false;
        }

        return true;
    }
}