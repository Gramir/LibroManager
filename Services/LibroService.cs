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
            foreach (var libro in libros)
            {
                // Load the ubicacion for each libro to ensure it's available for mapping
                if (libro.Ubicacion == null)
                {
                    var ubicacion = await _unitOfWork.Ubicaciones.GetByIdAsync(libro.UbicacionId);
                    libro.Ubicacion = ubicacion;
                }
            }
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
            if (libro == null)
            {
                return null;
            }

            if (libro.Ubicacion == null)
            {
                var ubicacion = await _unitOfWork.Ubicaciones.GetByIdAsync(libro.UbicacionId);
                libro.Ubicacion = ubicacion;
            }
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
                // Load the categoria and ubicacion for each libro
                if (libro.Categoria == null)
                {
                    var categoria = await _unitOfWork.Categorias.GetByIdAsync(libro.CategoriaId);
                    libro.Categoria = categoria;
                }
                if (libro.Ubicacion == null)
                {
                    var ubicacion = await _unitOfWork.Ubicaciones.GetByIdAsync(libro.UbicacionId);
                    libro.Ubicacion = ubicacion;
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
            // Obtener la ubicación antes de crear el libro
            var partes = libroDto.UbicacionString.Split('-');
            if (partes.Length != 3)
            {
                _logger.LogWarning("Formato de ubicación inválido: {Ubicacion}", libroDto.UbicacionString);
                return false;
            }

            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacion = ubicaciones.FirstOrDefault(u => 
                u.Estante == partes[0] && 
                u.Nivel == int.Parse(partes[1]) && 
                u.Posicion == int.Parse(partes[2]));

            if (ubicacion == null)
            {
                _logger.LogWarning("Ubicación no encontrada: {Ubicacion}", libroDto.UbicacionString);
                return false;
            }

            var libro = _mapper.Map<Libro>(libroDto);
            libro.UbicacionId = ubicacion.UbicacionId;
            
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

            // Generar un número de serie único para el primer ejemplar
            string serial;
            do
            {
                serial = GenerateSerial();
            } while (await _unitOfWork.Libros.SerialExistsAsync(serial));

            libro.Serial = serial;

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

    private string GenerateSerial()
    {
        // Formato: XXX-XXX-000 (letras mayúsculas, números y guiones)
        var random = new Random();
        var letters1 = new string(Enumerable.Range(0, 3)
            .Select(_ => (char)('A' + random.Next(26)))
            .ToArray());
        var letters2 = new string(Enumerable.Range(0, 3)
            .Select(_ => (char)('A' + random.Next(26)))
            .ToArray());
        var numbers = random.Next(1000).ToString("000");
        
        return $"{letters1}-{letters2}-{numbers}";
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

            // No permitimos cambiar el ISBN ni el Serial
            libroDto.ISBN = existingLibro.ISBN;
            libroDto.Serial = existingLibro.Serial;

            // Obtener la ubicación
            var partes = libroDto.UbicacionString.Split('-');
            if (partes.Length != 3)
            {
                _logger.LogWarning("Formato de ubicación inválido: {Ubicacion}", libroDto.UbicacionString);
                return false;
            }

            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacion = ubicaciones.FirstOrDefault(u => 
                u.Estante == partes[0] && 
                u.Nivel == int.Parse(partes[1]) && 
                u.Posicion == int.Parse(partes[2]));

            if (ubicacion == null)
            {
                _logger.LogWarning("Ubicación no encontrada: {Ubicacion}", libroDto.UbicacionString);
                return false;
            }

            // Actualizar solo los valores permitidos del libro existente
            existingLibro.Titulo = libroDto.Titulo;
            existingLibro.AutorId = libroDto.AutorId;
            existingLibro.CategoriaId = libroDto.CategoriaId;
            existingLibro.UbicacionId = ubicacion.UbicacionId;

            if (!await _validationService.LibroEsValido(existingLibro))
            {
                _logger.LogWarning("Datos de libro no válidos durante la actualización: {LibroId}", existingLibro.LibroId);
                return false;
            }

            _unitOfWork.Libros.Update(existingLibro);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Libro actualizado: {LibroId}, Título: {Titulo}", 
                existingLibro.LibroId, existingLibro.Titulo);
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

            // Verificar si el ejemplar específico está prestado
            if (await _unitOfWork.Libros.EstaPrestadoAsync(id))
            {
                _logger.LogWarning("No se puede eliminar el libro {LibroId} porque está prestado", id);
                return false;
            }

            // Obtener todos los ejemplares con el mismo ISBN
            var ejemplares = await _unitOfWork.Libros.GetAllAsync();
            var ejemplaresDelMismoLibro = ejemplares.Where(l => l.ISBN == libro.ISBN).ToList();

            // Si es el último ejemplar, verificar que no haya préstamos activos de ningún ejemplar
            if (ejemplaresDelMismoLibro.Count == 1)
            {
                var hayPrestamosActivos = ejemplaresDelMismoLibro.Any(l => l.EstaPrestado);
                if (hayPrestamosActivos)
                {
                    _logger.LogWarning("No se puede eliminar el último ejemplar del libro con ISBN {ISBN} porque hay préstamos activos", libro.ISBN);
                    return false;
                }
            }

            _unitOfWork.Libros.Remove(libro);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Libro eliminado: {LibroId}, ISBN: {ISBN}, Serial: {Serial}", 
                id, libro.ISBN, libro.Serial);
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
            var ejemplares = libros.Where(l => l.ISBN == isbn).ToList();
            
            // Cargar las ubicaciones para cada ejemplar
            foreach (var ejemplar in ejemplares)
            {
                if (ejemplar.Ubicacion == null)
                {
                    ejemplar.Ubicacion = await _unitOfWork.Ubicaciones.GetByIdAsync(ejemplar.UbicacionId);
                }
            }
            
            return _mapper.Map<IEnumerable<LibroDTO>>(ejemplares);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ejemplares para ISBN {ISBN}", isbn);
            return Enumerable.Empty<LibroDTO>();
        }
    }

    public async Task<bool> CreateEjemplarAsync(string isbn, string ubicacion)
    {
        try
        {
            // Parsear la ubicación (formato esperado: "A-1-1")
            var partes = ubicacion.Split('-');
            if (partes.Length != 3)
            {
                _logger.LogWarning("Formato de ubicación inválido: {Ubicacion}", ubicacion);
                return false;
            }

            // Buscar la ubicación en la base de datos
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacionEncontrada = ubicaciones.FirstOrDefault(u => 
                u.Estante == partes[0] && 
                u.Nivel == int.Parse(partes[1]) && 
                u.Posicion == int.Parse(partes[2]));

            if (ubicacionEncontrada == null)
            {
                _logger.LogWarning("Ubicación no encontrada: {Ubicacion}", ubicacion);
                return false;
            }

            // Obtener el libro original con sus detalles
            var ejemplares = await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
            var primerEjemplar = ejemplares.FirstOrDefault(l => l.ISBN == isbn);
            if (primerEjemplar == null)
            {
                _logger.LogWarning("No se encontró un libro con el ISBN {ISBN}", isbn);
                return false;
            }

            // Generar un número de serie único para el nuevo ejemplar
            string serial;
            do
            {
                serial = GenerateSerial();
            } while (await _unitOfWork.Libros.SerialExistsAsync(serial));

            // Crear el nuevo ejemplar con los mismos datos del libro original
            var nuevoEjemplar = new Libro
            {
                Titulo = primerEjemplar.Titulo,
                ISBN = isbn,
                Serial = serial,
                AutorId = primerEjemplar.AutorId,
                CategoriaId = primerEjemplar.CategoriaId,
                UbicacionId = ubicacionEncontrada.UbicacionId,
                Estado = EstadoLibro.Disponible,
                FechaCreacion = DateTime.Now
            };

            await _unitOfWork.Libros.AddAsync(nuevoEjemplar);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Nuevo ejemplar creado: ISBN: {ISBN}, Serial: {Serial}, Ubicacion: {Ubicacion}", 
                isbn, serial, ubicacion);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear nuevo ejemplar para ISBN {ISBN}", isbn);
            return false;
        }
    }
}