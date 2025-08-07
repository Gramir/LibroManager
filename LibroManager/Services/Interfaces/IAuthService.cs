using LibroManager.Models;

namespace LibroManager.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string[] Errors)> LoginAsync(string email, string password, bool rememberMe);
    Task<(bool Success, string[] Errors)> RegisterAsync(string email, string password, string nombreCompleto, string role);
    Task LogoutAsync();
    Task<ApplicationUser?> GetCurrentUserAsync();
    Task<bool> IsInRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    Task<(bool Success, string[] Errors)> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
}