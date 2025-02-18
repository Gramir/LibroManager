using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibroManager.Models;

public class Autor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AutorId { get; set; }
    
    [Required(ErrorMessage = "El nombre del autor es requerido")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navigation property
    public ICollection<Libro>? Libros { get; set; }
}