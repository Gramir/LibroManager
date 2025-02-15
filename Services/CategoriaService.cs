using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class CategoriaService : ICategoriaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private const int MAX_NOMBRE_LENGTH = 50;

    public CategoriaService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoriaDTO>> GetAllAsync()
    {
        try
        {
            var categorias = await _unitOfWork.Categorias.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        }
        catch
        {
            return Enumerable.Empty<CategoriaDTO>();
        }
    }

    public async Task<CategoriaDTO?> GetByIdAsync(int id)
    {
        try
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(id);
            return _mapper.Map<CategoriaDTO>(categoria);
        }
        catch
        {
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
        catch
        {
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
        catch
        {
            return Enumerable.Empty<CategoriaDTO>();
        }
    }

    public async Task<bool> CreateAsync(CategoriaCreateDTO categoriaDto)
    {
        try
        {
            if (!ValidateCategoriaData(categoriaDto))
                return false;

            if (await _unitOfWork.Categorias.GetByNombreAsync(categoriaDto.Nombre) != null)
                return false;

            var categoria = _mapper.Map<Categoria>(categoriaDto);
            await _unitOfWork.Categorias.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(CategoriaUpdateDTO categoriaDto)
    {
        try
        {
            if (!ValidateCategoriaData(categoriaDto))
                return false;

            var existingCategoria = await _unitOfWork.Categorias.GetByIdAsync(categoriaDto.CategoriaId);
            if (existingCategoria == null)
                return false;

            var categoria = _mapper.Map<Categoria>(categoriaDto);
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

            _unitOfWork.Categorias.Remove(categoria);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool ValidateCategoriaData(CategoriaCreateDTO categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Nombre) || categoria.Nombre.Length > MAX_NOMBRE_LENGTH)
            return false;

        return true;
    }
}