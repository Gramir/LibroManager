using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibroManager.Models;

public class Libro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LibroId { get; set; }
    
    [Required(ErrorMessage = "El título del libro es requerido")]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El ISBN es requerido")]
    [StringLength(13)]
    public string ISBN { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "El autor es requerido")]
    public int AutorId { get; set; }
    
    [Required(ErrorMessage = "La categoría es requerida")]
    public int CategoriaId { get; set; }

    // Navigation properties
    [ForeignKey("AutorId")]
    public Autor? Autor { get; set; }
    
    [ForeignKey("CategoriaId")]
    public Categoria? Categoria { get; set; }
    
    public ICollection<Prestamo>? Prestamos { get; set; }
}