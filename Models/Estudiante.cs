using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Models;

[Index(nameof(Email), IsUnique = true)]
public class Estudiante
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EstudianteId { get; set; }
    
    [Required(ErrorMessage = "El nombre del estudiante es requerido")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "Por favor, ingrese una dirección de correo electrónico válida")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "La fecha de inscripción es requerida")]
    public DateTime FechaInscripcion { get; set; }

    // Navigation property
    public ICollection<Prestamo>? Prestamos { get; set; }
}