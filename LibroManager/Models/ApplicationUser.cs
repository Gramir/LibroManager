using Microsoft.AspNetCore.Identity;

namespace LibroManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string NombreCompleto { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}