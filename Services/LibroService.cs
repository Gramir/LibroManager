using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class LibroService : ILibroService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILibroValidationService _validationService;
    private readonly IMapper _mapper;

    public LibroService(IUnitOfWork unitOfWork, ILibroValidationService validationService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _validationService = validationService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LibroDTO>> GetAllLibrosAsync()
    {
        var libros = await _unitOfWork.Libros.GetLibrosWithAutorAndCategoriaAsync();
        return _mapper.Map<IEnumerable<LibroDTO>>(libros);
    }

    public async Task<LibroDTO?> GetLibroByIdAsync(int id)
    {
        var libro = await _unitOfWork.Libros.GetLibroWithDetailsAsync(id);
        return _mapper.Map<LibroDTO>(libro);
    }

    public async Task<bool> CreateLibroAsync(LibroCreateDTO libroDto)
    {
        var libro = _mapper.Map<Libro>(libroDto);
        
        if (!await _validationService.LibroEsValido(libro))
            return false;

        if (await _unitOfWork.Libros.IsbnExistsAsync(libro.ISBN))
            return false;

        await _unitOfWork.Libros.AddAsync(libro);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateLibroAsync(LibroUpdateDTO libroDto)
    {
        var existingLibro = await _unitOfWork.Libros.GetByIdAsync(libroDto.LibroId);
        if (existingLibro == null)
            return false;

        // Verificar si el ISBN ha cambiado y si ya existe
        if (existingLibro.ISBN != libroDto.ISBN && await _unitOfWork.Libros.IsbnExistsAsync(libroDto.ISBN))
            return false;

        // Actualizar los valores del libro existente
        existingLibro.Titulo = libroDto.Titulo;
        existingLibro.ISBN = libroDto.ISBN;
        existingLibro.AutorId = libroDto.AutorId;
        existingLibro.CategoriaId = libroDto.CategoriaId;
        existingLibro.NumeroEjemplares = libroDto.NumeroEjemplares;

        if (!await _validationService.LibroEsValido(existingLibro))
            return false;

        _unitOfWork.Libros.Update(existingLibro);
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