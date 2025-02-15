using System.ComponentModel.DataAnnotations;

namespace LibroManager.DTOs;

public class CategoriaDTO
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CantidadLibros { get; set; }
}

public class CategoriaCreateDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
    public string Nombre { get; set; } = string.Empty;
}

public class CategoriaUpdateDTO : CategoriaCreateDTO
{
    public int CategoriaId { get; set; }
}