using Microsoft.EntityFrameworkCore;

namespace LibroManager.Services;

public class ValidationService
{
    public static string GetUniqueValidationMessage(string field)
    {
        return field switch
        {
            "ISBN" => "Ya existe un libro con este ISBN",
            "Nombre" => "Ya existe una categoría con este nombre",
            "Email" => "Ya existe un estudiante con este correo electrónico",
            _ => "El valor ingresado ya existe en la base de datos"
        };
    }

    public static string HandleDbUpdateException(DbUpdateException ex)
    {
        if (ex.InnerException?.Message.Contains("IX_Libros_ISBN") ?? false)
            return GetUniqueValidationMessage("ISBN");
        if (ex.InnerException?.Message.Contains("IX_Categorias_Nombre") ?? false)
            return GetUniqueValidationMessage("Nombre");
        if (ex.InnerException?.Message.Contains("IX_Estudiantes_Email") ?? false)
            return GetUniqueValidationMessage("Email");

        return "Error al guardar los cambios en la base de datos";
    }
}