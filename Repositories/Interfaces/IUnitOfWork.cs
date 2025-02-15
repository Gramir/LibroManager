namespace LibroManager.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ILibroRepository Libros { get; }
    IAutorRepository Autores { get; }
    Task<int> SaveChangesAsync();
}