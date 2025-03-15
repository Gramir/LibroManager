using LibroManager.Models;
using Microsoft.AspNetCore.Identity;

namespace LibroManager.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
    Task<ApplicationUser?> FindByIdAsync(string id);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<IList<string>> GetRolesAsync(ApplicationUser user);
    Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
    Task<IdentityResult> UpdateAsync(ApplicationUser user);
    Task<IdentityResult> DeleteAsync(ApplicationUser user);
    Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);
    Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles);
}