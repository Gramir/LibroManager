using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibroManager.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
    Task<Dictionary<string, List<string>>> GetAllRolesWithPermissionsAsync();
    Task<IdentityRole?> GetRoleByIdAsync(string id);
    Task<IdentityResult> CreateRoleAsync(string roleName, IEnumerable<string> permissions);
    Task<IdentityResult> UpdateRoleAsync(IdentityRole role, IEnumerable<string> permissions);
    Task<IdentityResult> DeleteRoleAsync(string roleId);
    Task<bool> CanDeleteRoleAsync(string roleId);
    Task<IList<Claim>> GetRolePermissionsAsync(string roleId);
}