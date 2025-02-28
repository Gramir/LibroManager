using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;

namespace LibroManager.Repositories;

public class UbicacionRepository : GenericRepository<Ubicacion>, IUbicacionRepository
{
    public UbicacionRepository(ApplicationDbContext context) : base(context)
    {
    }
}