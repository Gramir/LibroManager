using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibroManager.Services;

public class LibroService : ILibroService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILibroValidationService _validationService;
    private readonly IMapper _mapper;
    private readonly ILogger<LibroService> _logger;

    public LibroService(
        IUnitOfWork unitOfWork, 
        ILibroValidationService validationService, 
        IMapper mapper,
        ILogger<LibroService> logger)
    {
        _unitOfWork = unitOfWork;
        _validationService = validationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<LibroDTO>> GetAllLibrosAsync()
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
            return _mapper.Map<IEnumerable<LibroDTO>>(libros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los libros");
            return Enumerable.Empty<LibroDTO>();
        }
    }

    public async Task<LibroDTO?> GetLibroByIdAsync(int id)
    {
        try
        {
            var libro = await _unitOfWork.Libros.GetLibroWithDetailsAsync(id);
            return _mapper.Map<LibroDTO>(libro);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener libro con ID {LibroId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<LibroDTO>> GetLibrosByAutorIdAsync(int autorId)
    {
        try
        {
            var autor = await _unitOfWork.Autores.GetAutorWithLibrosAsync(autorId);
            if (autor?.Libros == null)
            {
                _logger.LogWarning("No se encontraron libros para el autor con ID {AutorId}", autorId);
                return Array.Empty<LibroDTO>();
            }
                
            foreach (var libro in autor.Libros)
            {
                // Load the categoria for each libro to ensure it's available for mapping
                if (libro.Categoria == null)
                {
                    var categoria = await _unitOfWork.Categorias.GetByIdAsync(libro.CategoriaId);
                    libro.Categoria = categoria;
                }
            }

            return _mapper.Map<IEnumerable<LibroDTO>>(autor.Libros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener libros del autor {AutorId}", autorId);
            return Enumerable.Empty<LibroDTO>();
        }
    }

    public async Task<bool> CreateLibroAsync(LibroCreateDTO libroDto)
    {
        try
        {
            var libro = _mapper.Map<Libro>(libroDto);
            
            if (!await _validationService.LibroEsValido(libro))
            {
                _logger.LogWarning("Datos de libro no válidos durante la creación: {LibroData}", 
                    new { libro.AutorId, libro.CategoriaId });
                return false;
            }

            if (await _unitOfWork.Libros.IsbnExistsAsync(libro.ISBN))
            {
                _logger.LogWarning("ISBN ya existe: {ISBN}", libro.ISBN);
                return false;
            }

            if (await _unitOfWork.Libros.SerialExistsAsync(libro.Serial))
            {
                _logger.LogWarning("Serial ya existe: {Serial}", libro.Serial);
                return false;
            }

            await _unitOfWork.Libros.AddAsync(libro);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Libro creado: {LibroId}, Título: {Titulo}, ISBN: {ISBN}, Serial: {Serial}", 
                libro.LibroId, libro.Titulo, libro.ISBN, libro.Serial);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear libro");
            return false;
        }
    }

    public async Task<bool> UpdateLibroAsync(LibroUpdateDTO libroDto)
    {
        try
        {
            var existingLibro = await _unitOfWork.Libros.GetByIdAsync(libroDto.LibroId);
            if (existingLibro == null)
            {
                _logger.LogWarning("Libro no encontrado para actualización: {LibroId}", libroDto.LibroId);
                return false;
            }

            // Verificar si el ISBN ha cambiado y si ya existe
            if (existingLibro.ISBN != libroDto.ISBN && await _unitOfWork.Libros.IsbnExistsAsync(libroDto.ISBN))
            {
                _logger.LogWarning("ISBN ya existe en otro libro: {ISBN}", libroDto.ISBN);
                return false;
            }

            // Verificar si el Serial ha cambiado y si ya existe
            if (existingLibro.Serial != libroDto.Serial && await _unitOfWork.Libros.SerialExistsAsync(libroDto.Serial))
            {
                _logger.LogWarning("Serial ya existe en otro libro: {Serial}", libroDto.Serial);
                return false;
            }

            // Actualizar los valores del libro existente
            existingLibro.Titulo = libroDto.Titulo;
            existingLibro.ISBN = libroDto.ISBN;
            existingLibro.Serial = libroDto.Serial;
            existingLibro.AutorId = libroDto.AutorId;
            existingLibro.CategoriaId = libroDto.CategoriaId;
            existingLibro.Ubicacion = libroDto.Ubicacion;

            if (!await _validationService.LibroEsValido(existingLibro))
            {
                _logger.LogWarning("Datos de libro no válidos durante la actualización: {LibroId}", existingLibro.LibroId);
                return false;
            }

            _unitOfWork.Libros.Update(existingLibro);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Libro actualizado: {LibroId}, Título: {Titulo}, ISBN: {ISBN}, Serial: {Serial}", 
                existingLibro.LibroId, existingLibro.Titulo, existingLibro.ISBN, existingLibro.Serial);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar libro {LibroId}", libroDto.LibroId);
            return false;
        }
    }

    public async Task<bool> DeleteLibroAsync(int id)
    {
        try
        {
            var libro = await _unitOfWork.Libros.GetByIdAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("Libro no encontrado para eliminación: {LibroId}", id);
                return false;
            }

            // Verificar primero si el ejemplar está prestado
            if (await _unitOfWork.Libros.EstaPrestadoAsync(id))
            {
                _logger.LogWarning("No se puede eliminar el libro {LibroId} porque está prestado", id);
                return false;
            }

            // Obtener todos los ejemplares con el mismo ISBN
            var ejemplares = await _unitOfWork.Libros.GetAllAsync();
            var cantidadEjemplares = ejemplares.Count(l => l.ISBN == libro.ISBN);

            // Si es el último ejemplar y hay préstamos activos de cualquier ejemplar, no permitir la eliminación
            if (cantidadEjemplares == 1 && ejemplares.Any(l => l.ISBN == libro.ISBN && l.EstaPrestado))
            {
                _logger.LogWarning("No se puede eliminar el último ejemplar del libro con ISBN {ISBN} porque hay préstamos activos", libro.ISBN);
                return false;
            }

            _unitOfWork.Libros.Remove(libro);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Libro eliminado: {LibroId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar libro {LibroId}", id);
            return false;
        }
    }

    public async Task<bool> ExisteIsbnAsync(string isbn)
    {
        try
        {
            return await _unitOfWork.Libros.IsbnExistsAsync(isbn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de ISBN {ISBN}", isbn);
            return false;
        }
    }

    public async Task<bool> ExisteSerialAsync(string serial)
    {
        try
        {
            return await _unitOfWork.Libros.SerialExistsAsync(serial);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de Serial {Serial}", serial);
            return false;
        }
    }
    
    public async Task<int> ContarEjemplaresPorIsbnAsync(string isbn)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetAllAsync();
            return libros.Count(l => l.ISBN == isbn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al contar ejemplares para ISBN {ISBN}", isbn);
            return 0;
        }
    }

    public async Task<IEnumerable<LibroDTO>> GetEjemplaresPorIsbnAsync(string isbn)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetAllAsync();
            var ejemplares = libros.Where(l => l.ISBN == isbn);
            return _mapper.Map<IEnumerable<LibroDTO>>(ejemplares);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ejemplares para ISBN {ISBN}", isbn);
            return Enumerable.Empty<LibroDTO>();
        }
    }
}