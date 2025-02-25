using AutoMapper;
using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibroManager.Services;

public class AutorService : IAutorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AutorService> _logger;

    public AutorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AutorService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AutorDTO>> GetAllAsync()
    {
        try
        {
            var autores = await _unitOfWork.Autores.GetAllAsync();
            return _mapper.Map<IEnumerable<AutorDTO>>(autores);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los autores");
            return Enumerable.Empty<AutorDTO>();
        }
    }

    public async Task<AutorDTO?> GetByIdAsync(int id)
    {
        try
        {
            var autor = await _unitOfWork.Autores.GetAutorWithLibrosAsync(id);
            return _mapper.Map<AutorDTO>(autor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener autor con ID {AutorId}", id);
            return null;
        }
    }

    public async Task<bool> CreateAsync(AutorCreateDTO autorDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(autorDto.Nombre))
            {
                _logger.LogWarning("Intento de crear autor con nombre vacío");
                return false;
            }

            var autor = _mapper.Map<Autor>(autorDto);
            await _unitOfWork.Autores.AddAsync(autor);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Autor creado: {AutorId}, Nombre: {Nombre}", 
                autor.AutorId, autor.Nombre);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear autor: {Nombre}", autorDto.Nombre);
            return false;
        }
    }

    public async Task<bool> UpdateAsync(AutorUpdateDTO autorDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(autorDto.Nombre))
            {
                _logger.LogWarning("Intento de actualizar autor con nombre vacío: {AutorId}", autorDto.AutorId);
                return false;
            }

            var existingAutor = await _unitOfWork.Autores.GetByIdAsync(autorDto.AutorId);
            if (existingAutor == null)
            {
                _logger.LogWarning("Autor no encontrado para actualización: {AutorId}", autorDto.AutorId);
                return false;
            }

            existingAutor.Nombre = autorDto.Nombre;
            _unitOfWork.Autores.Update(existingAutor);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Autor actualizado: {AutorId}, Nombre: {Nombre}", 
                existingAutor.AutorId, existingAutor.Nombre);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar autor {AutorId}", autorDto.AutorId);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var autor = await _unitOfWork.Autores.GetByIdAsync(id);
            if (autor == null)
            {
                _logger.LogWarning("Autor no encontrado para eliminación: {AutorId}", id);
                return false;
            }

            if (await _unitOfWork.Autores.HasLibrosAsync(id))
            {
                _logger.LogWarning("No se puede eliminar autor {AutorId} porque tiene libros asociados", id);
                return false;
            }

            _unitOfWork.Autores.Remove(autor);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Autor eliminado: {AutorId}, Nombre: {Nombre}", 
                autor.AutorId, autor.Nombre);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar autor {AutorId}", id);
            return false;
        }
    }
}