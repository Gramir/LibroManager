using LibroManager.DTOs;

namespace LibroManager.Services.Interfaces;

public interface ILibroService
{
    Task<IEnumerable<LibroDTO>> GetAllLibrosAsync();
    Task<LibroDTO?> GetLibroByIdAsync(int id);
    Task<IEnumerable<LibroDTO>> GetLibrosByAutorIdAsync(int autorId);
    Task<bool> CreateLibroAsync(LibroCreateDTO libro);
    Task<bool> UpdateLibroAsync(LibroUpdateDTO libro);
    Task<(bool puedeEliminar, bool tienePrestamosHistoricos)> PuedeEliminarLibroAsync(int id);
    Task<bool> DeleteLibroAsync(int id, bool eliminarHistorial = false);
    Task<bool> ExisteIsbnAsync(string isbn);
    Task<bool> ExisteSerialAsync(string serial);
    Task<int> ContarEjemplaresPorIsbnAsync(string isbn);
    Task<int> ContarEjemplaresPrestadosPorIsbnAsync(string isbn);
    Task<int> ContarEjemplaresPerdidosPorIsbnAsync(string isbn);
    Task<IEnumerable<LibroDTO>> GetEjemplaresPorIsbnAsync(string isbn);
    Task<bool> CreateEjemplarAsync(string isbn, string ubicacion);
    Task<IEnumerable<LibroDTO>> GetLibrosPorUbicacionAsync(int ubicacionId);
    Task<IEnumerable<LibroDTO>> GetLibrosPorCategoriaAsync(int categoriaId);
    Task<bool> UpdateEjemplaresCompartidosAsync(string isbn, int autorId, int categoriaId);
    Task<bool> UpdateEjemplarAsync(int libroId, string ubicacionNueva);
    Task<string> GetNextAvailableSerial(string isbn);
}