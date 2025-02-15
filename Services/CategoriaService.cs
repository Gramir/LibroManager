using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class CategoriaService : ICategoriaService
{
    private readonly IUnitOfWork _unitOfWork;
    private const int MAX_NOMBRE_LENGTH = 50;

    public CategoriaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Categoria>> GetAllAsync()
    {
        try
        {
            return await _unitOfWork.Categorias.GetAllAsync();
        }
        catch
        {
            return Enumerable.Empty<Categoria>();
        }
    }

    public async Task<Categoria?> GetByIdAsync(int id)
    {
        try
        {
            return await _unitOfWork.Categorias.GetByIdAsync(id);
        }
        catch
        {
            return null;
        }
    }

    public async Task<Categoria?> GetByNombreAsync(string nombre)
    {
        try
        {
            if (!ValidateNombre(nombre))
                return null;

            return await _unitOfWork.Categorias.GetByNombreAsync(nombre);
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasWithLibrosAsync()
    {
        try
        {
            return await _unitOfWork.Categorias.GetCategoriasWithLibrosAsync();
        }
        catch
        {
            return Enumerable.Empty<Categoria>();
        }
    }

    public async Task<bool> CreateAsync(Categoria categoria)
    {
        try
        {
            if (!ValidateCategoriaData(categoria))
                return false;

            if (await _unitOfWork.Categorias.GetByNombreAsync(categoria.Nombre) != null)
                return false;

            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(Categoria categoria)
    {
        try
        {
            if (!ValidateCategoriaData(categoria))
                return false;

            var categoriaConMismoNombre = await _unitOfWork.Categorias.GetByNombreAsync(categoria.Nombre);
            if (categoriaConMismoNombre != null && categoriaConMismoNombre.CategoriaId != categoria.CategoriaId)
            {
                return false;
            }

            var existingCategoria = await _unitOfWork.Categorias.GetByIdAsync(categoria.CategoriaId);
            if (existingCategoria == null)
                return false;

            _unitOfWork.Categorias.Update(categoria);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);
            if (categoria == null)
                return false;

            if (categoria.Libros?.Any() == true)
                return false;

            _unitOfWork.Categorias.Remove(categoria);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    private bool ValidateCategoriaData(Categoria categoria)
    {
        if (categoria == null)
            return false;

        return ValidateNombre(categoria.Nombre);
    }

    private bool ValidateNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return false;

        if (nombre.Length > MAX_NOMBRE_LENGTH)
            return false;

        return true;
    }
}