namespace LibroManager.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ILibroRepository Libros { get; }
    IAutorRepository Autores { get; }
    IPrestamoRepository Prestamos { get; }
    IEstudianteRepository Estudiantes { get; }
    ICategoriaRepository Categorias { get; }
    IUbicacionRepository Ubicaciones { get; }
    Task<int> SaveChangesAsync();
}