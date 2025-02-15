using AutoMapper;
using LibroManager.DTOs;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;

namespace LibroManager.Services;

public class PrestamoService : IPrestamoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PrestamoService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PrestamoDTO>> GetAllAsync()
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetAllAsync();
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch
        {
            return Enumerable.Empty<PrestamoDTO>();
        }
    }

    public async Task<PrestamoDTO?> GetByIdAsync(int id)
    {
        try
        {
            var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
            return _mapper.Map<PrestamoDTO>(prestamo);
        }
        catch
        {
            return null;
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosByEstudianteAsync(int estudianteId)
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(estudianteId);
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch
        {
            return Enumerable.Empty<PrestamoDTO>();
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosByLibroAsync(int libroId)
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libroId);
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch
        {
            return Enumerable.Empty<PrestamoDTO>();
        }
    }

    public async Task<IEnumerable<PrestamoDTO>> GetPrestamosActivosAsync()
    {
        try
        {
            var prestamos = await _unitOfWork.Prestamos.GetPrestamosActivosAsync();
            return _mapper.Map<IEnumerable<PrestamoDTO>>(prestamos);
        }
        catch
        {
            return Enumerable.Empty<PrestamoDTO>();
        }
    }

    public async Task<bool> CreateAsync(PrestamoCreateDTO prestamoDto)
    {
        try
        {
            var prestamo = _mapper.Map<Prestamo>(prestamoDto);
            
            if (!ValidatePrestamoData(prestamo))
                return false;

            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            if (libro == null)
                return false;

            var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(prestamo.LibroId);
            if (prestamosActivos.Any(p => p.FechaVencimiento >= DateTime.Now))
                return false;

            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(prestamo.EstudianteId);
            if (estudiante == null)
                return false;

            var prestamosEstudiante = await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId);
            if (prestamosEstudiante.Any(p => p.FechaVencimiento < DateTime.Now))
                return false;

            await _unitOfWork.Prestamos.AddAsync(prestamo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateAsync(PrestamoUpdateDTO prestamoDto)
    {
        try
        {
            var prestamo = _mapper.Map<Prestamo>(prestamoDto);
            
            if (!ValidatePrestamoData(prestamo))
                return false;

            var existingPrestamo = await _unitOfWork.Prestamos.GetByIdAsync(prestamo.PrestamoId);
            if (existingPrestamo == null)
                return false;

            prestamo.LibroId = existingPrestamo.LibroId;
            prestamo.EstudianteId = existingPrestamo.EstudianteId;
            prestamo.FechaPrestamo = existingPrestamo.FechaPrestamo;

            _unitOfWork.Prestamos.Update(prestamo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var prestamo = await _unitOfWork.Prestamos.GetByIdAsync(id);
            if (prestamo == null)
                return false;

            _unitOfWork.Prestamos.Remove(prestamo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private bool ValidatePrestamoData(Prestamo prestamo)
    {
        if (prestamo.FechaPrestamo > prestamo.FechaVencimiento)
            return false;

        if (prestamo.FechaPrestamo > DateTime.Now)
            return false;

        if (prestamo.FechaVencimiento < DateTime.Now)
            return false;

        return true;
    }
}