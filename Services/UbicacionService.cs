using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class UbicacionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UbicacionService> logger) : IUbicacionService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UbicacionService> _logger = logger;

    public async Task<IEnumerable<UbicacionDTO>> GetAllUbicacionesAsync()
    {
        try
        {
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacionesDto = _mapper.Map<IEnumerable<UbicacionDTO>>(ubicaciones);

            foreach (var ubicacion in ubicacionesDto)
            {
                ubicacion.EstaDisponible = !ubicaciones.Any(u =>
                    u.UbicacionId == ubicacion.UbicacionId &&
                    u.Libros != null &&
                    u.Libros.Count > 0);
            }

            return ubicacionesDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las ubicaciones");
            return [];
        }
    }

    public async Task<UbicacionDTO?> GetUbicacionByIdAsync(int id)
    {
        try
        {
            var ubicacion = await _unitOfWork.Ubicaciones.GetByIdAsync(id);
            if (ubicacion == null) return null;

            var ubicacionDto = _mapper.Map<UbicacionDTO>(ubicacion);
            ubicacionDto.EstaDisponible = ubicacion.Libros == null || ubicacion.Libros.Count == 0;
            return ubicacionDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ubicación con ID {UbicacionId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<UbicacionDTO>> GetUbicacionesByEstanteAsync(string estante)
    {
        try
        {
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacionesFiltradas = ubicaciones.Where(u => u.Estante.Equals(estante, StringComparison.OrdinalIgnoreCase));
            var ubicacionesDto = _mapper.Map<IEnumerable<UbicacionDTO>>(ubicacionesFiltradas);

            foreach (var ubicacion in ubicacionesDto)
            {
                var original = ubicaciones.First(u => u.UbicacionId == ubicacion.UbicacionId);
                ubicacion.EstaDisponible = original.Libros == null || original.Libros.Count == 0;
            }

            return ubicacionesDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ubicaciones del estante {Estante}", estante);
            return [];
        }
    }

    public async Task<IEnumerable<UbicacionDTO>> GetUbicacionesByEstanteAndNivelAsync(string estante, int nivel)
    {
        try
        {
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacionesFiltradas = ubicaciones.Where(u =>
                u.Estante.Equals(estante, StringComparison.OrdinalIgnoreCase) &&
                u.Nivel == nivel);
            var ubicacionesDto = _mapper.Map<IEnumerable<UbicacionDTO>>(ubicacionesFiltradas);

            foreach (var ubicacion in ubicacionesDto)
            {
                var original = ubicaciones.First(u => u.UbicacionId == ubicacion.UbicacionId);
                ubicacion.EstaDisponible = original.Libros == null || original.Libros.Count == 0;
            }

            return ubicacionesDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ubicaciones del estante {Estante} y nivel {Nivel}", estante, nivel);
            return [];
        }
    }

    public async Task<IEnumerable<UbicacionDTO>> GetAvailableUbicacionesAsync()
    {
        try
        {
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var ubicacionesDisponibles = ubicaciones.Where(u => u.Libros == null || u.Libros.Count == 0);
            var ubicacionesDto = _mapper.Map<IEnumerable<UbicacionDTO>>(ubicacionesDisponibles);

            foreach (var ubicacion in ubicacionesDto)
            {
                ubicacion.EstaDisponible = true;
            }

            return ubicacionesDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ubicaciones disponibles");
            return [];
        }
    }

    public async Task<bool> CreateUbicacionAsync(UbicacionCreateDTO ubicacionDto)
    {
        try
        {
            var ubicacion = _mapper.Map<Ubicacion>(ubicacionDto);
            await _unitOfWork.Ubicaciones.AddAsync(ubicacion);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Ubicación creada: Estante {Estante}, Nivel {Nivel}, Posición {Posicion}",
                ubicacion.Estante, ubicacion.Nivel, ubicacion.Posicion);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear ubicación");
            return false;
        }
    }

    public async Task<bool> UpdateUbicacionAsync(int id, UbicacionUpdateDTO ubicacionDto)
    {
        try
        {
            var ubicacionExistente = await _unitOfWork.Ubicaciones.GetByIdAsync(id);
            if (ubicacionExistente == null)
            {
                _logger.LogWarning("No se encontró la ubicación con ID {UbicacionId} para actualizar", id);
                return false;
            }

            _mapper.Map(ubicacionDto, ubicacionExistente);
            _unitOfWork.Ubicaciones.Update(ubicacionExistente);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Ubicación actualizada: ID {UbicacionId}, Estante {Estante}, Nivel {Nivel}, Posición {Posicion}",
                id, ubicacionExistente.Estante, ubicacionExistente.Nivel, ubicacionExistente.Posicion);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar ubicación {UbicacionId}", id);
            return false;
        }
    }

    public async Task<bool> DeleteUbicacionAsync(int id)
    {
        try
        {
            var ubicacion = await _unitOfWork.Ubicaciones.GetByIdAsync(id);
            if (ubicacion == null)
            {
                _logger.LogWarning("No se encontró la ubicación con ID {UbicacionId} para eliminar", id);
                return false;
            }

            if (ubicacion.Libros != null && ubicacion.Libros.Count > 0)
            {
                _logger.LogWarning("No se puede eliminar la ubicación {UbicacionId} porque tiene libros asociados", id);
                return false;
            }

            _unitOfWork.Ubicaciones.Remove(ubicacion);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Ubicación eliminada: ID {UbicacionId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar ubicación {UbicacionId}", id);
            return false;
        }
    }

    public async Task<bool> UbicacionExistsAsync(int id)
    {
        try
        {
            var ubicacion = await _unitOfWork.Ubicaciones.GetByIdAsync(id);
            return ubicacion != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de ubicación {UbicacionId}", id);
            return false;
        }
    }

    public async Task<IEnumerable<UbicacionDTO>> GetAvailableUbicacionesWithCurrentAsync(int libroId)
    {
        try
        {
            var ubicaciones = await _unitOfWork.Ubicaciones.GetAllAsync();
            var libros = await _unitOfWork.Libros.GetAllAsync();

            var libroActual = libros.FirstOrDefault(l => l.LibroId == libroId);
            if (libroActual == null)
            {
                _logger.LogWarning("No se encontró el libro con ID {LibroId}", libroId);
                return [];
            }

            var ubicacionesOcupadas = libros
                .Where(l => l.LibroId != libroId)
                .Select(l => l.UbicacionId)
                .ToHashSet();

            var ubicacionesDisponibles = ubicaciones
                .Where(u => !ubicacionesOcupadas.Contains(u.UbicacionId) || u.UbicacionId == libroActual.UbicacionId)
                .ToList();

            var ubicacionesDto = _mapper.Map<List<UbicacionDTO>>(ubicacionesDisponibles);

            // Marcar ubicación actual como no disponible y el resto como disponibles
            foreach (var ubicacionDto in ubicacionesDto)
            {
                ubicacionDto.EstaDisponible = ubicacionDto.UbicacionId != libroActual.UbicacionId;
            }

            _logger.LogInformation("Se encontraron {Count} ubicaciones disponibles para el libro {LibroId}",
                ubicacionesDisponibles.Count, libroId);
            return ubicacionesDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener ubicaciones disponibles para el libro {LibroId}", libroId);
            return [];
        }
    }
}