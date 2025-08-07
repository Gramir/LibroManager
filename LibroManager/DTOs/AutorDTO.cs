using System.ComponentModel.DataAnnotations;

namespace LibroManager.DTOs;

public class AutorDTO
{
    public int AutorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CantidadLibros { get; set; }
}

public class AutorCreateDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;
}

public class AutorUpdateDTO : AutorCreateDTO
{
    public int AutorId { get; set; }
}