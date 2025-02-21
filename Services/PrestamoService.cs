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
    private readonly ILibroValidationService _libroValidationService;

    public PrestamoService(IUnitOfWork unitOfWork, IMapper mapper, ILibroValidationService libroValidationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _libroValidationService = libroValidationService;
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
            prestamo.FechaPrestamo = DateTime.Now;
            prestamo.Estado = EstadoPrestamo.Activo;
            prestamo.FechaCreacion = DateTime.Now;

            // Validate dates
            if (!_libroValidationService.FechasPrestamoSonValidas(prestamo.FechaPrestamo, prestamo.FechaVencimiento))
                return false;

            // Get libro and estudiante
            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(prestamo.EstudianteId);
            
            if (libro == null || estudiante == null)
                return false;

            // Check for expired prestamos
            var prestamosEstudiante = await _unitOfWork.Prestamos.GetPrestamosByEstudianteAsync(prestamo.EstudianteId);
            if (prestamosEstudiante.Any(p => p.Estado == EstadoPrestamo.Expirado))
                return false;

            // Check ejemplares availability
            var prestamosActivos = await _unitOfWork.Prestamos.GetPrestamosByLibroAsync(libro.LibroId);
            var prestamosCount = prestamosActivos.Count(p => p.Estado == EstadoPrestamo.Activo);
            
            if (prestamosCount >= libro.NumeroEjemplares)
                return false;
            
            prestamo.Estado = EstadoPrestamo.Activo;
            
            // Update libro estado only if all ejemplares will be borrowed
            if (prestamosCount + 1 >= libro.NumeroEjemplares)
            {
                libro.Estado = EstadoLibro.Prestado;
                _unitOfWork.Libros.Update(libro);
            }
            
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
            
            var existingPrestamo = await _unitOfWork.Prestamos.GetByIdAsync(prestamo.PrestamoId);
            if (existingPrestamo == null)
                return false;

            // Mantener los valores originales que no deben cambiar
            prestamo.LibroId = existingPrestamo.LibroId;
            prestamo.EstudianteId = existingPrestamo.EstudianteId;
            prestamo.FechaPrestamo = existingPrestamo.FechaPrestamo;
            prestamo.FechaCreacion = existingPrestamo.FechaCreacion;

            if (!ValidatePrestamoData(prestamo))
                return false;

            var libro = await _unitOfWork.Libros.GetByIdAsync(prestamo.LibroId);
            if (libro == null)
                return false;

            // Actualizar estado del libro según el estado del préstamo
            if (prestamo.FechaDevolucion.HasValue)
            {
                prestamo.Estado = EstadoPrestamo.Concluido;
                libro.Estado = EstadoLibro.Disponible;
            }
            else if (prestamo.Estado == EstadoPrestamo.Expirado)
            {
                libro.Estado = EstadoLibro.Perdido;
            }

            _unitOfWork.Libros.Update(libro);
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
        // Normalizamos las fechas para comparar solo las fechas sin tiempo
        var fechaPrestamo = prestamo.FechaPrestamo.Date;
        var fechaVencimiento = prestamo.FechaVencimiento.Date;
        var hoy = DateTime.Today;

        if (fechaPrestamo > fechaVencimiento)
            return false;

        if (fechaPrestamo > hoy)
            return false;

        // Si la fecha de vencimiento es pasada, marcar como expirado
        if (fechaVencimiento < hoy && prestamo.Estado != EstadoPrestamo.Concluido)
        {
            prestamo.Estado = EstadoPrestamo.Expirado;
        }

        if (fechaVencimiento > fechaPrestamo.AddDays(30))
            return false;

        if (prestamo.FechaDevolucion.HasValue)
        {
            if (prestamo.FechaDevolucion.Value < fechaPrestamo)
                return false;
            
            prestamo.Estado = EstadoPrestamo.Concluido;
        }

        return true;
    }
}