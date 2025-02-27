using System.ComponentModel.DataAnnotations;

namespace LibroManager.DTOs;

public class LibroDTO
{
    public int LibroId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public string Serial { get; set; } = string.Empty;
    public string AutorNombre { get; set; } = string.Empty;
    public string CategoriaNombre { get; set; } = string.Empty;
    public bool EstaPrestado { get; set; }
    public string Ubicacion { get; set; } = string.Empty;
}

public class LibroCreateDTO
{
    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ISBN es requerido")]
    [StringLength(13, MinimumLength = 10, ErrorMessage = "El ISBN debe tener entre 10 y 13 caracteres")]
    [RegularExpression(@"^[0-9-]*$", ErrorMessage = "El ISBN solo puede contener números y guiones")]
    public string ISBN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un autor")]
    [Range(1, int.MaxValue, ErrorMessage = "Por favor, seleccione un autor de la lista")]
    public int AutorId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una categoría")]
    [Range(1, int.MaxValue, ErrorMessage = "Por favor, seleccione una categoría de la lista")]
    public int CategoriaId { get; set; }

    [Required(ErrorMessage = "La ubicación es requerida")]
    [StringLength(100, ErrorMessage = "La ubicación no puede exceder los 100 caracteres")]
    public string Ubicacion { get; set; } = string.Empty;
}

public class LibroUpdateDTO : LibroCreateDTO
{
    public int LibroId { get; set; }
    public string Serial { get; set; } = string.Empty; // Solo para mostrar en el formulario de edición, no se puede modificar
}