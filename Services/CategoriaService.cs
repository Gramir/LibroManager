using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class CategoriaService : ICategoriaService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Categoria>> GetAllAsync()
    {
        return await _unitOfWork.Categorias.GetAllAsync();
    }

    public async Task<Categoria?> GetByIdAsync(int id)
    {
        return await _unitOfWork.Categorias.GetByIdAsync(id);
    }

    public async Task<Categoria?> GetByNombreAsync(string nombre)
    {
        return await _unitOfWork.Categorias.GetByNombreAsync(nombre);
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasWithLibrosAsync()
    {
        return await _unitOfWork.Categorias.GetCategoriasWithLibrosAsync();
    }

    public async Task<bool> CreateAsync(Categoria categoria)
    {
        try
        {
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
            var existingCategoria = await _unitOfWork.Categorias.GetByNombreAsync(categoria.Nombre);
            if (existingCategoria != null && existingCategoria.CategoriaId != categoria.CategoriaId)
                return false;

            _unitOfWork.Categorias.Update(categoria);
            await _unitOfWork.SaveChangesAsync();
            return true;
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

            var categoriaWithLibros = await _unitOfWork.Categorias.GetByIdAsync(id);
            if (categoriaWithLibros?.Libros?.Any() == true)
                return false;

            _unitOfWork.Categorias.Remove(categoria);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}