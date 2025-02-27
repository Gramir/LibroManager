using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Models;

public enum EstadoLibro
{
    Disponible,
    Prestado,
    Perdido
}

[Index(nameof(Ubicacion), IsUnique = true)]
[Index(nameof(Serial), IsUnique = true)]
public class Libro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LibroId { get; set; }
    
    [Required(ErrorMessage = "El título del libro es requerido")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El ISBN es requerido")]
    [StringLength(13, MinimumLength = 10, ErrorMessage = "El ISBN debe tener entre 10 y 13 caracteres")]
    [RegularExpression(@"^[0-9-]*$", ErrorMessage = "El ISBN solo puede contener números y guiones")]
    public string ISBN { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El número de serie es requerido")]
    [StringLength(50)]
    [RegularExpression(@"^[A-Z0-9-]*$", ErrorMessage = "El Serial solo puede contener letras mayúsculas, números y guiones")]
    public string Serial { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El autor es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un autor válido")]
    public int AutorId { get; set; }
    
    [Required(ErrorMessage = "La categoría es requerida")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida")]
    public int CategoriaId { get; set; }

    [Required]
    public EstadoLibro Estado { get; set; } = EstadoLibro.Disponible;
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "La ubicación es requerida")]
    [StringLength(100, ErrorMessage = "La ubicación no puede exceder los 100 caracteres")]
    public string Ubicacion { get; set; } = string.Empty;

    [NotMapped]
    public bool EstaPrestado => Estado == EstadoLibro.Prestado;
    
    
    public static int GetCantidadEjemplares(IEnumerable<Libro> libros, string isbn)
    {
        return libros?.Count(l => l.ISBN == isbn) ?? 0;
    }

    // Navigation properties
    [ForeignKey("AutorId")]
    public Autor? Autor { get; set; }
    
    [ForeignKey("CategoriaId")]
    public Categoria? Categoria { get; set; }
    
    public ICollection<Prestamo>? Prestamos { get; set; }
}