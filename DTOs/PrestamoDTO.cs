using System.ComponentModel.DataAnnotations;
using LibroManager.Models;

namespace LibroManager.DTOs;

public class PrestamoDTO
{
    public int PrestamoId { get; set; }
    public int LibroId { get; set; }
    public int EstudianteId { get; set; }
    public string LibroTitulo { get; set; } = string.Empty;
    public string EstudianteNombre { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public bool EstaActivo { get; set; }
}

public class PrestamoCreateDTO
{
    [Required(ErrorMessage = "El libro es requerido")]
    public int LibroId { get; set; }

    [Required(ErrorMessage = "El estudiante es requerido")]
    public int EstudianteId { get; set; }

    [Required(ErrorMessage = "La fecha de vencimiento es requerida")]
    [DataType(DataType.Date)]
    public DateTime FechaVencimiento { get; set; }
}

public class PrestamoUpdateDTO
{
    public int PrestamoId { get; set; }
    
    [Required(ErrorMessage = "El libro es requerido")]
    public int LibroId { get; set; }

    [Required(ErrorMessage = "El estudiante es requerido")]
    public int EstudianteId { get; set; }

    [Required(ErrorMessage = "La fecha de préstamo es requerida")]
    [DataType(DataType.Date)]
    public DateTime FechaPrestamo { get; set; }
    
    [Required(ErrorMessage = "La fecha de vencimiento es requerida")]
    [DataType(DataType.Date)]
    public DateTime FechaVencimiento { get; set; }
    
    public DateTime? FechaDevolucion { get; set; }
    
    public EstadoPrestamo Estado { get; set; }
}