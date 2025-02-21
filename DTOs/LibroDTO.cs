using System.ComponentModel.DataAnnotations;

namespace LibroManager.DTOs;

public class LibroDTO
{
    public int LibroId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string AutorNombre { get; set; } = string.Empty;
    public string CategoriaNombre { get; set; } = string.Empty;
    public int NumeroEjemplares { get; set; }
    public bool EstaPrestado { get; set; }
}

public class LibroCreateDTO
{
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ISBN es requerido")]
    [StringLength(13, ErrorMessage = "El ISBN no puede exceder los 13 caracteres")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "El autor es requerido")]
    public int AutorId { get; set; }

    [Required(ErrorMessage = "La categoría es requerida")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "El número de ejemplares es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El número de ejemplares debe ser mayor a 0")]
    public int NumeroEjemplares { get; set; } = 1;
}

public class LibroUpdateDTO : LibroCreateDTO
{
    public int LibroId { get; set; }
}