using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace LibroManager.Services.Interfaces;

public interface IRoleService
{
    Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
    Task<Dictionary<string, List<string>>> GetAllRolesWithPermissionsAsync();
    Task<IList<Claim>> GetRolePermissionsAsync(string roleId);
    Task EnsureDefaultRolesExistAsync();
}