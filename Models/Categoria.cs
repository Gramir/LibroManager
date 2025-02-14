using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibroManager.Models;

public class Categoria
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoriaId { get; set; }
    
    [Required(ErrorMessage = "El nombre de la categoría es requerido")]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    // Navigation property
    public ICollection<Libro>? Libros { get; set; }
}