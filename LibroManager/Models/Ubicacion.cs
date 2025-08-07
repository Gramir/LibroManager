using System.ComponentModel.DataAnnotations;

namespace LibroManager.Models;

public class Ubicacion
{
    [Key]
    public int UbicacionId { get; set; }

    [Required(ErrorMessage = "El estante es requerido")]
    [RegularExpression(@"^[A-Z]$", ErrorMessage = "El estante debe ser una letra mayúscula (A-Z)")]
    public string Estante { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nivel es requerido")]
    [Range(1, 10, ErrorMessage = "El nivel debe estar entre 1 y 10")]
    public int Nivel { get; set; }

    [Required(ErrorMessage = "La posición es requerida")]
    [Range(1, 50, ErrorMessage = "La posición debe estar entre 1 y 50")]
    public int Posicion { get; set; }

    // Propiedad de navegación
    public ICollection<Libro>? Libros { get; set; }

    // Método para obtener la ubicación en formato legible
    public string ObtenerUbicacionFormateada()
    {
        return $"{Estante}-{Nivel}-{Posicion}";
    }
}