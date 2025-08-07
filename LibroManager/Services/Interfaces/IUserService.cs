using LibroManager.Models;
using Microsoft.AspNetCore.Identity;

namespace LibroManager.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<Dictionary<string, List<string>>> GetAllUsersWithRolesAsync();
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<(IdentityResult Result, ApplicationUser? User)> CreateUserAsync(ApplicationUser user, string password, IEnumerable<string> roles);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles);
    Task<IdentityResult> DeleteUserAsync(string userId);
}