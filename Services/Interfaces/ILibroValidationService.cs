using LibroManager.Models;

namespace LibroManager.Services.Interfaces;

public interface ILibroValidationService
{
    Task<bool> LibroEsValido(Libro libro);
    Task<bool> PuedeEliminarAutor(int autorId);
    Task<bool> PuedeEliminarCategoria(int categoriaId);
    Task<bool> LibroEstaPrestado(int libroId);
    Task<bool> HayEjemplaresDisponibles(int libroId);
    Task<bool> PrestamoEsValido(Prestamo prestamo);
    bool FechasPrestamoSonValidas(DateTime fechaPrestamo, DateTime fechaVencimiento);
}