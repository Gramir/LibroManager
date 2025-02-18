using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibroManager.Models;

public enum EstadoPrestamo
{
    Activo,
    Concluido,
    Expirado
}

public class Prestamo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PrestamoId { get; set; }
    
    [Required(ErrorMessage = "El libro es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un libro válido")]
    public int LibroId { get; set; }
    
    [Required(ErrorMessage = "El estudiante es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estudiante válido")]
    public int EstudianteId { get; set; }
    
    [Required(ErrorMessage = "La fecha de préstamo es requerida")]
    [DataType(DataType.Date)]
    public DateTime FechaPrestamo { get; set; }
    
    [Required(ErrorMessage = "La fecha de vencimiento es requerida")]
    [DataType(DataType.Date)]
    [CustomValidation(typeof(Prestamo), nameof(ValidateFechaVencimiento))]
    public DateTime FechaVencimiento { get; set; }

    [Required]
    public EstadoPrestamo Estado { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? FechaDevolucion { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [NotMapped]
    public bool EstaActivo => Estado == EstadoPrestamo.Activo;

    // Navigation properties
    [ForeignKey("LibroId")]
    public Libro? Libro { get; set; }
    
    [ForeignKey("EstudianteId")]
    public Estudiante? Estudiante { get; set; }

    public static ValidationResult? ValidateFechaVencimiento(DateTime fechaVencimiento, ValidationContext context)
    {
        var prestamo = (Prestamo)context.ObjectInstance;
        if (fechaVencimiento <= prestamo.FechaPrestamo)
        {
            return new ValidationResult("La fecha de vencimiento debe ser posterior a la fecha de préstamo");
        }
        return ValidationResult.Success;
    }
}