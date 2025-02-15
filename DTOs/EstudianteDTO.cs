using System.ComponentModel.DataAnnotations;

namespace LibroManager.DTOs;

public class EstudianteDTO
{
    public int EstudianteId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public int PrestamosActivos { get; set; }
}

public class EstudianteCreateDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string Email { get; set; } = string.Empty;
}

public class EstudianteUpdateDTO : EstudianteCreateDTO
{
    public int EstudianteId { get; set; }
}