using LibroManager.Data.Context;
using LibroManager.Repositories.Interfaces;

namespace LibroManager.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private ILibroRepository? _libroRepository;
    private IAutorRepository? _autorRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ILibroRepository Libros
    {
        get
        {
            _libroRepository ??= new LibroRepository(_context);
            return _libroRepository;
        }
    }

    public IAutorRepository Autores
    {
        get
        {
            _autorRepository ??= new AutorRepository(_context);
            return _autorRepository;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}