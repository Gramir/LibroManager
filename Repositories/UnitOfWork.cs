using LibroManager.Data.Context;
using LibroManager.Repositories.Interfaces;

namespace LibroManager.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private ILibroRepository? _libroRepository;
    private IAutorRepository? _autorRepository;
    private IPrestamoRepository? _prestamoRepository;
    private IEstudianteRepository? _estudianteRepository;
    private ICategoriaRepository? _categoriaRepository;
    private IUbicacionRepository? _ubicacionRepository;

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

    public IPrestamoRepository Prestamos => 
        _prestamoRepository ??= new PrestamoRepository(_context);

    public IEstudianteRepository Estudiantes =>
        _estudianteRepository ??= new EstudianteRepository(_context);

    public ICategoriaRepository Categorias =>
        _categoriaRepository ??= new CategoriaRepository(_context);

    public IUbicacionRepository Ubicaciones =>
        _ubicacionRepository ??= new UbicacionRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}