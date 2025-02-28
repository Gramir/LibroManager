using LibroManager.DTOs;

namespace LibroManager.Services.Interfaces;

public interface IUbicacionService
{
    Task<IEnumerable<UbicacionDTO>> GetAllUbicacionesAsync();
    Task<UbicacionDTO?> GetUbicacionByIdAsync(int id);
    Task<IEnumerable<UbicacionDTO>> GetUbicacionesByEstanteAsync(string estante);
    Task<IEnumerable<UbicacionDTO>> GetUbicacionesByEstanteAndNivelAsync(string estante, int nivel);
    Task<IEnumerable<UbicacionDTO>> GetAvailableUbicacionesAsync();
    Task<bool> CreateUbicacionAsync(UbicacionCreateDTO ubicacion);
    Task<bool> UpdateUbicacionAsync(int id, UbicacionUpdateDTO ubicacion);
    Task<bool> DeleteUbicacionAsync(int id);
    Task<bool> UbicacionExistsAsync(int id);
}