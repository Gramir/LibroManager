using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibroManager.Models;

public class Prestamo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PrestamoId { get; set; }
    
    [Required(ErrorMessage = "El libro es requerido")]
    public int LibroId { get; set; }
    
    [Required(ErrorMessage = "El estudiante es requerido")]
    public int EstudianteId { get; set; }
    
    [Required(ErrorMessage = "La fecha de préstamo es requerida")]
    public DateTime FechaPrestamo { get; set; }
    
    [Required(ErrorMessage = "La fecha de vencimiento es requerida")]
    public DateTime FechaVencimiento { get; set; }

    // Navigation properties
    [ForeignKey("LibroId")]
    public Libro? Libro { get; set; }
    
    [ForeignKey("EstudianteId")]
    public Estudiante? Estudiante { get; set; }
}