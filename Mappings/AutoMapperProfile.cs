using AutoMapper;
using LibroManager.Models;
using LibroManager.DTOs;

namespace LibroManager.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Mapeos de Libro
        CreateMap<Libro, LibroDTO>()
            .ForMember(dest => dest.AutorNombre, opt => opt.MapFrom(src => src.Autor != null ? src.Autor.Nombre : string.Empty))
            .ForMember(dest => dest.CategoriaNombre, opt => opt.MapFrom(src => src.Categoria != null ? src.Categoria.Nombre : string.Empty))
            .ForMember(dest => dest.UbicacionFormateada, opt => opt.MapFrom(src => src.Ubicacion != null ? src.Ubicacion.ObtenerUbicacionFormateada() : string.Empty))
            .ForMember(dest => dest.EstaPrestado, opt => opt.MapFrom(src => src.Estado == EstadoLibro.Prestado))
            .ForMember(dest => dest.EstaPerdido, opt => opt.MapFrom(src => src.Estado == EstadoLibro.Perdido));

        CreateMap<LibroCreateDTO, Libro>()
            .ForMember(dest => dest.UbicacionId, opt => opt.Ignore());

        CreateMap<LibroUpdateDTO, Libro>()
            .ForMember(dest => dest.UbicacionId, opt => opt.Ignore());

        // Mapeos de Autor
        CreateMap<Autor, AutorDTO>()
            .ForMember(dest => dest.CantidadLibros, opt => opt.MapFrom(src => src.Libros != null ? src.Libros.Count : 0));
        CreateMap<AutorCreateDTO, Autor>();
        CreateMap<AutorUpdateDTO, Autor>();

        // Mapeos de Categoria
        CreateMap<Categoria, CategoriaDTO>()
            .ForMember(dest => dest.CantidadLibros, opt => opt.MapFrom(src => src.Libros != null ? src.Libros.Count : 0));
        CreateMap<CategoriaCreateDTO, Categoria>();
        CreateMap<CategoriaUpdateDTO, Categoria>();

        // Mapeos de Estudiante
        CreateMap<Estudiante, EstudianteDTO>()
            .ForMember(dest => dest.PrestamosActivos, 
                opt => opt.MapFrom(src => src.Prestamos != null ? 
                    src.Prestamos.Count(p => p.FechaVencimiento >= DateTime.Now) : 0));
        CreateMap<EstudianteCreateDTO, Estudiante>();
        CreateMap<EstudianteUpdateDTO, Estudiante>();

        // Mapeos de Prestamo
        CreateMap<Prestamo, PrestamoDTO>()
            .ForMember(dest => dest.LibroTitulo, opt => opt.MapFrom(src => src.Libro != null ? src.Libro.Titulo : string.Empty))
            .ForMember(dest => dest.EstudianteNombre, opt => opt.MapFrom(src => src.Estudiante != null ? src.Estudiante.Nombre : string.Empty))
            .ForMember(dest => dest.EstaActivo, opt => opt.MapFrom(src => 
                src.Estado == EstadoPrestamo.Activo && 
                !src.FechaDevolucion.HasValue));
        CreateMap<PrestamoCreateDTO, Prestamo>()
            .ForMember(dest => dest.FechaPrestamo, opt => opt.MapFrom(src => DateTime.Now));
        CreateMap<PrestamoUpdateDTO, Prestamo>();
        
        // Mapeos de Ubicacion
        CreateMap<Ubicacion, UbicacionDTO>()
            .ForMember(dest => dest.EstaDisponible, opt => opt.MapFrom(src => src.Libros == null || !src.Libros.Any()))
            .ForMember(dest => dest.UbicacionFormateada, opt => opt.MapFrom(src => src.ObtenerUbicacionFormateada()));
        CreateMap<UbicacionCreateDTO, Ubicacion>();
        CreateMap<UbicacionUpdateDTO, Ubicacion>();
    }
}