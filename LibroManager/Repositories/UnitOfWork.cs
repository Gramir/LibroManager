using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LibroManager.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private ILibroRepository? _libroRepository;
    private IAutorRepository? _autorRepository;
    private IPrestamoRepository? _prestamoRepository;
    private IEstudianteRepository? _estudianteRepository;
    private ICategoriaRepository? _categoriaRepository;
    private IUbicacionRepository? _ubicacionRepository;
    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;

    public ILibroRepository Libros => _libroRepository ??= new LibroRepository(_context);
    public IAutorRepository Autores => _autorRepository ??= new AutorRepository(_context);
    public IPrestamoRepository Prestamos => _prestamoRepository ??= new PrestamoRepository(_context);
    public IEstudianteRepository Estudiantes => _estudianteRepository ??= new EstudianteRepository(_context);
    public ICategoriaRepository Categorias => _categoriaRepository ??= new CategoriaRepository(_context);
    public IUbicacionRepository Ubicaciones => _ubicacionRepository ??= new UbicacionRepository(_context);
    public IUserRepository Users => _userRepository ??= new UserRepository(_userManager);
    public IRoleRepository Roles => _roleRepository ??= new RoleRepository(_roleManager, _userManager);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
        }
    }
}