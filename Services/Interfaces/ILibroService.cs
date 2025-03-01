using LibroManager.DTOs;

namespace LibroManager.Services.Interfaces;

public interface ILibroService
{
    Task<IEnumerable<LibroDTO>> GetAllLibrosAsync();
    Task<LibroDTO?> GetLibroByIdAsync(int id);
    Task<IEnumerable<LibroDTO>> GetLibrosByAutorIdAsync(int autorId);
    Task<bool> CreateLibroAsync(LibroCreateDTO libro);
    Task<bool> UpdateLibroAsync(LibroUpdateDTO libro);
    Task<bool> DeleteLibroAsync(int id);
    Task<bool> ExisteIsbnAsync(string isbn);
    Task<bool> ExisteSerialAsync(string serial);
    Task<int> ContarEjemplaresPorIsbnAsync(string isbn);
    Task<IEnumerable<LibroDTO>> GetEjemplaresPorIsbnAsync(string isbn);
    Task<bool> CreateEjemplarAsync(string isbn, string ubicacion);
    Task<IEnumerable<LibroDTO>> GetLibrosPorUbicacionAsync(int ubicacionId);
}