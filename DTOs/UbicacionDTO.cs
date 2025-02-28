using System.ComponentModel.DataAnnotations;

namespace LibroManager.DTOs;

public class UbicacionDTO
{
    public int UbicacionId { get; set; }
    public string Estante { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public int Posicion { get; set; }
    public string UbicacionFormateada => $"{Estante}-{Nivel}-{Posicion}";
    public bool EstaDisponible { get; set; }
}

public class UbicacionCreateDTO
{
    [Required(ErrorMessage = "El estante es requerido")]
    [RegularExpression(@"^[A-Z]$", ErrorMessage = "El estante debe ser una letra mayúscula (A-Z)")]
    public string Estante { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nivel es requerido")]
    [Range(1, 10, ErrorMessage = "El nivel debe estar entre 1 y 10")]
    public int Nivel { get; set; }

    [Required(ErrorMessage = "La posición es requerida")]
    [Range(1, 50, ErrorMessage = "La posición debe estar entre 1 y 50")]
    public int Posicion { get; set; }
}

public class UbicacionUpdateDTO
{
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
}