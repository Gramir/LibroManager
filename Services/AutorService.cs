using LibroManager.Models;
using LibroManager.Repositories.Interfaces;

namespace LibroManager.Services;

public class AutorService
{
    private readonly IUnitOfWork _unitOfWork;

    public AutorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Autor>> GetAllAutoresAsync()
    {
        return await _unitOfWork.Autores.GetAllAsync();
    }

    public async Task<Autor?> GetAutorByIdAsync(int id)
    {
        return await _unitOfWork.Autores.GetAutorWithLibrosAsync(id);
    }

    public async Task<bool> CreateAutorAsync(Autor autor)
    {
        if (string.IsNullOrWhiteSpace(autor.Nombre))
            return false;

        await _unitOfWork.Autores.AddAsync(autor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAutorAsync(Autor autor)
    {
        if (string.IsNullOrWhiteSpace(autor.Nombre))
            return false;

        var existingAutor = await _unitOfWork.Autores.GetByIdAsync(autor.AutorId);
        if (existingAutor == null)
            return false;

        _unitOfWork.Autores.Update(autor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAutorAsync(int id)
    {
        var autor = await _unitOfWork.Autores.GetByIdAsync(id);
        if (autor == null)
            return false;

        if (await _unitOfWork.Autores.HasLibrosAsync(id))
            return false;

        _unitOfWork.Autores.Remove(autor);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}