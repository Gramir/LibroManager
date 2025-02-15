using LibroManager.Models;
using LibroManager.Repositories.Interfaces;

namespace LibroManager.Services;

public class LibroService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly LibroValidationService _validationService;

    public LibroService(IUnitOfWork unitOfWork, LibroValidationService validationService)
    {
        _unitOfWork = unitOfWork;
        _validationService = validationService;
    }

    public async Task<IEnumerable<Libro>> GetAllLibrosAsync()
    {
        return await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
    }

    public async Task<Libro?> GetLibroByIdAsync(int id)
    {
        return await _unitOfWork.Libros.GetLibroWithDetailsAsync(id);
    }

    public async Task<bool> CreateLibroAsync(Libro libro)
    {
        if (!await _validationService.LibroEsValido(libro))
            return false;

        if (await _unitOfWork.Libros.IsbnExistsAsync(libro.ISBN))
            return false;

        await _unitOfWork.Libros.AddAsync(libro);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateLibroAsync(Libro libro)
    {
        if (!await _validationService.LibroEsValido(libro))
            return false;

        var existingLibro = await _unitOfWork.Libros.GetByIdAsync(libro.LibroId);
        if (existingLibro == null)
            return false;

        if (existingLibro.ISBN != libro.ISBN && await _unitOfWork.Libros.IsbnExistsAsync(libro.ISBN))
            return false;

        _unitOfWork.Libros.Update(libro);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLibroAsync(int id)
    {
        var libro = await _unitOfWork.Libros.GetByIdAsync(id);
        if (libro == null)
            return false;

        if (await _unitOfWork.Libros.EstaPrestadoAsync(id))
            return false;

        _unitOfWork.Libros.Remove(libro);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExisteIsbnAsync(string isbn)
    {
        return await _unitOfWork.Libros.IsbnExistsAsync(isbn);
    }
}