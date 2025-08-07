using LibroManager.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibroManager.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
    Task<IdentityRole?> FindByIdAsync(string id);
    Task<IdentityRole?> FindByNameAsync(string name);
    Task<IdentityResult> CreateAsync(IdentityRole role);
    Task<IdentityResult> UpdateAsync(IdentityRole role);
    Task<IdentityResult> DeleteAsync(IdentityRole role);
    Task<IList<Claim>> GetClaimsAsync(IdentityRole role);
    Task<IdentityResult> AddClaimAsync(IdentityRole role, Claim claim);
    Task<IdentityResult> RemoveClaimAsync(IdentityRole role, Claim claim);
    Task<bool> RoleExistsAsync(string roleName);
    Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName);
}