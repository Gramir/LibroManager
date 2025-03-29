using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class LibroService(
    IUnitOfWork unitOfWork,
    ILibroValidationService validationService,
    IMapper mapper,
    ILogger<LibroService> logger) : ILibroService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILibroValidationService _validationService = validationService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<LibroService> _logger = logger;

    public async Task<IEnumerable<LibroDTO>> GetAllLibrosAsync()
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
            foreach (var libro in libros)
            {
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
            return [];
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
                return [];
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
            return [];
        }
    }

    public async Task<bool> CreateLibroAsync(LibroCreateDTO libroDto)
    {
        try
        {
            if (!await ValidarUbicacionSeleccionada(libroDto.UbicacionString))
            {
                _logger.LogWarning("Ubicación no válida o no seleccionada del listado: {Ubicacion}", libroDto.UbicacionString);
                return false;
            }

            // Buscar la ubicación en la base de datos
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacion = ubicaciones.FirstOrDefault(u => u.ObtenerUbicacionFormateada() == libroDto.UbicacionString);

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

            // Generar el serial para el primer ejemplar (será ISBN-1)
            libro.Serial = $"{libro.ISBN}-1";

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

    public async Task<string> GetNextAvailableSerial(string isbn)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetAllAsync();
            var ejemplaresDelIsbn = libros.Where(l => l.ISBN == isbn).ToList();
            
            var numerosSerialExistentes = ejemplaresDelIsbn
                .Select(l => int.Parse(l.Serial[(l.Serial.LastIndexOf('-') + 1)..]))
                .OrderBy(n => n)
                .ToList();

            int nuevoNumero = 1;
            foreach (var numero in numerosSerialExistentes)
            {
                if (numero != nuevoNumero)
                {
                    break;
                }
                nuevoNumero++;
            }

            return $"{isbn}-{nuevoNumero}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar serial para ISBN {ISBN}", isbn);
            throw;
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

            // Actualizar solo los valores permitidos del libro existente
            existingLibro.Titulo = libroDto.Titulo;
            existingLibro.AutorId = libroDto.AutorId;
            existingLibro.CategoriaId = libroDto.CategoriaId;

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

    public async Task<(bool puedeEliminar, bool tienePrestamosHistoricos)> PuedeEliminarLibroAsync(int id)
    {
        try
        {
            var libro = await _unitOfWork.Libros.GetByIdAsync(id);
            if (libro == null)
                return (false, false);

            var prestamos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(id);
            var tienePrestamosActivos = prestamos.Any(p => p.Estado == EstadoPrestamo.Activo);
            var tienePrestamosHistoricos = prestamos.Any(p => p.Estado != EstadoPrestamo.Activo);

            return (!tienePrestamosActivos, tienePrestamosHistoricos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si se puede eliminar el libro {LibroId}", id);
            return (false, false);
        }
    }

    public async Task<bool> DeleteLibroAsync(int id, bool eliminarHistorial = false)
    {
        try
        {
            var libro = await _unitOfWork.Libros.GetByIdAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("Libro no encontrado para eliminación: {LibroId}", id);
                return false;
            }

            var prestamos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(id);
            var tienePrestamosActivos = prestamos.Any(p => p.Estado == EstadoPrestamo.Activo);

            if (tienePrestamosActivos)
            {
                _logger.LogWarning("No se puede eliminar el libro {LibroId} porque tiene préstamos activos", id);
                return false;
            }

            if (eliminarHistorial)
            {
                var resultado = await _unitOfWork.Prestamos.EliminarHistorialPrestamosAsync(id);
                if (!resultado)
                {
                    _logger.LogError("Error al eliminar el historial de préstamos del libro {LibroId}", id);
                    return false;
                }
            }
            else
            {
                var tienePrestamosHistoricos = prestamos.Any(p => p.Estado != EstadoPrestamo.Activo);
                if (tienePrestamosHistoricos)
                {
                    _logger.LogWarning("No se puede eliminar el libro {LibroId} porque tiene préstamos históricos", id);
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

    public async Task<int> ContarEjemplaresPrestadosPorIsbnAsync(string isbn)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetAllAsync();
            return libros.Count(l => l.ISBN == isbn && l.Estado == EstadoLibro.Prestado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al contar ejemplares prestados para ISBN {ISBN}", isbn);
            return 0;
        }
    }

    public async Task<int> ContarEjemplaresPerdidosPorIsbnAsync(string isbn)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetAllAsync();
            return libros.Count(l => l.ISBN == isbn && l.Estado == EstadoLibro.Perdido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al contar ejemplares perdidos para ISBN {ISBN}", isbn);
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
                ejemplar.Ubicacion ??= await _unitOfWork.Ubicaciones.GetByIdAsync(ejemplar.UbicacionId);
            }

            return _mapper.Map<IEnumerable<LibroDTO>>(ejemplares);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ejemplares para ISBN {ISBN}", isbn);
            return [];
        }
    }

    public async Task<bool> CreateEjemplarAsync(string isbn, string ubicacion)
    {
        try
        {
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacionEncontrada = ubicaciones.FirstOrDefault(u => u.ObtenerUbicacionFormateada() == ubicacion);

            if (ubicacionEncontrada == null)
            {
                _logger.LogWarning("Ubicación no encontrada: {Ubicacion}", ubicacion);
                return false;
            }

            var ejemplares = await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
            var primerEjemplar = ejemplares.FirstOrDefault(l => l.ISBN == isbn);
            if (primerEjemplar == null)
            {
                _logger.LogWarning("No se encontró un libro con el ISBN {ISBN}", isbn);
                return false;
            }

            var serial = await GetNextAvailableSerial(isbn);

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

    public async Task<IEnumerable<LibroDTO>> GetLibrosPorUbicacionAsync(int ubicacionId)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
            var librosEnUbicacion = libros.Where(l => l.UbicacionId == ubicacionId).ToList();

            foreach (var libro in librosEnUbicacion)
            {
                libro.Ubicacion ??= await _unitOfWork.Ubicaciones.GetByIdAsync(libro.UbicacionId);
            }

            return _mapper.Map<IEnumerable<LibroDTO>>(librosEnUbicacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener libros para la ubicación {UbicacionId}", ubicacionId);
            return [];
        }
    }

    private async Task<bool> ValidarUbicacionSeleccionada(string ubicacionString)
    {
        var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
        return ubicaciones.Any(u => u.ObtenerUbicacionFormateada() == ubicacionString);
    }

    public async Task<IEnumerable<LibroDTO>> GetLibrosPorCategoriaAsync(int categoriaId)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
            var librosPorCategoria = libros.Where(l => l.CategoriaId == categoriaId).ToList();

            // Cargar las ubicaciones para cada libro
            foreach (var libro in librosPorCategoria)
            {
                libro.Ubicacion ??= await _unitOfWork.Ubicaciones.GetByIdAsync(libro.UbicacionId);
            }

            return _mapper.Map<IEnumerable<LibroDTO>>(librosPorCategoria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener libros para la categoría {CategoriaId}", categoriaId);
            return [];
        }
    }

    public async Task<bool> UpdateEjemplaresCompartidosAsync(string isbn, int autorId, int categoriaId)
    {
        try
        {
            var ejemplares = await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
            var ejemplaresDelMismoLibro = ejemplares.Where(l => l.ISBN == isbn).ToList();

            if (ejemplaresDelMismoLibro.Count == 0)
            {
                _logger.LogWarning("No se encontraron ejemplares con el ISBN {ISBN}", isbn);
                return false;
            }

            foreach (var ejemplar in ejemplaresDelMismoLibro)
            {
                ejemplar.AutorId = autorId;
                ejemplar.CategoriaId = categoriaId;

                if (!await _validationService.LibroEsValido(ejemplar))
                {
                    _logger.LogWarning("Datos de libro no válidos durante la actualización masiva: {LibroId}", ejemplar.LibroId);
                    return false;
                }

                _unitOfWork.Libros.Update(ejemplar);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Actualización masiva completada para ISBN {ISBN}. Ejemplares actualizados: {Count}",
                isbn, ejemplaresDelMismoLibro.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar ejemplares compartidos para ISBN {ISBN}", isbn);
            return false;
        }
    }

    public async Task<bool> UpdateEjemplarAsync(int libroId, string ubicacionNueva)
    {
        try
        {
            var ejemplar = await _unitOfWork.Libros.GetByIdAsync(libroId);
            if (ejemplar == null)
            {
                _logger.LogWarning("Ejemplar no encontrado para actualización: {LibroId}", libroId);
                return false;
            }

            // Buscar la ubicación en la base de datos
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacionEncontrada = ubicaciones.FirstOrDefault(u => u.ObtenerUbicacionFormateada() == ubicacionNueva);

            if (ubicacionEncontrada == null)
            {
                _logger.LogWarning("Ubicación no encontrada: {Ubicacion}", ubicacionNueva);
                return false;
            }

            ejemplar.UbicacionId = ubicacionEncontrada.UbicacionId;
            _unitOfWork.Libros.Update(ejemplar);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Ejemplar actualizado: LibroId: {LibroId}, Nueva Ubicacion: {Ubicacion}",
                libroId, ubicacionNueva);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar ejemplar {LibroId}", libroId);
            return false;
        }
    }
}