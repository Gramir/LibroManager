using Microsoft.AspNetCore.Identity;
using System;

namespace LibroManager.Models
{
    public class ApplicationUser : IdentityUser
    {
        public required string NombreCompleto { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}