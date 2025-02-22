using AutoMapper;
using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class AutorService : IAutorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AutorService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AutorDTO>> GetAllAsync()
    {
        var autores = await _unitOfWork.Autores.GetAllAsync();
        return _mapper.Map<IEnumerable<AutorDTO>>(autores);
    }

    public async Task<AutorDTO?> GetByIdAsync(int id)
    {
        var autor = await _unitOfWork.Autores.GetAutorWithLibrosAsync(id);
        return _mapper.Map<AutorDTO>(autor);
    }

    public async Task<bool> CreateAsync(AutorCreateDTO autorDto)
    {
        if (string.IsNullOrWhiteSpace(autorDto.Nombre))
            return false;

        var autor = _mapper.Map<Autor>(autorDto);
        await _unitOfWork.Autores.AddAsync(autor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(AutorUpdateDTO autorDto)
    {
        if (string.IsNullOrWhiteSpace(autorDto.Nombre))
            return false;

        var autor = _mapper.Map<Autor>(autorDto);
        var existingAutor = await _unitOfWork.Autores.GetByIdAsync(autor.AutorId);
        if (existingAutor == null)
            return false;

        _unitOfWork.Autores.Update(autor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var autor = await _unitOfWork.Autores.GetByIdAsync(id);
        if (autor == null)
            return false;

        if (await _unitOfWork.Autores.HasLibrosAsync(id))
            return false;

        _unitOfWork.Autores.Remove(autor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}