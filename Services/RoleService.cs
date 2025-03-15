using System.Security.Claims;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LibroManager.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
    {
        return await _unitOfWork.Roles.GetAllRolesAsync();
    }

    public async Task<Dictionary<string, List<string>>> GetAllRolesWithPermissionsAsync()
    {
        var roles = await GetAllRolesAsync();
        var rolesWithPermissions = new Dictionary<string, List<string>>();

        foreach (var role in roles)
        {
            var claims = await _unitOfWork.Roles.GetClaimsAsync(role);
            rolesWithPermissions[role.Id] = claims.Select(c => c.Value).ToList();
        }

        return rolesWithPermissions;
    }

    public async Task<IdentityRole?> GetRoleByIdAsync(string roleId)
    {
        return await _unitOfWork.Roles.FindByIdAsync(roleId);
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName, IEnumerable<string> permissions)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Role name cannot be empty" });
        }

        var role = new IdentityRole(roleName);
        var result = await _unitOfWork.Roles.CreateAsync(role);

        if (result.Succeeded)
        {
            foreach (var permission in permissions)
            {
                await _unitOfWork.Roles.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }

        return result;
    }

    public async Task<IdentityResult> DeleteRoleAsync(string roleId)
    {
        var role = await _unitOfWork.Roles.FindByIdAsync(roleId);
        if (role == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Role not found" });
        }

        return await _unitOfWork.Roles.DeleteAsync(role);
    }

    public async Task<IdentityResult> UpdateRoleAsync(IdentityRole role, IEnumerable<string> permissions)
    {
        var existingRole = await _unitOfWork.Roles.FindByIdAsync(role.Id);
        if (existingRole == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Role not found" });
        }

        existingRole.Name = role.Name;
        var result = await _unitOfWork.Roles.UpdateAsync(existingRole);

        if (result.Succeeded)
        {
            var currentClaims = await _unitOfWork.Roles.GetClaimsAsync(existingRole);
            
            // Eliminar permisos actuales
            foreach (var claim in currentClaims)
            {
                await _unitOfWork.Roles.RemoveClaimAsync(existingRole, claim);
            }

            // Agregar nuevos permisos
            foreach (var permission in permissions)
            {
                await _unitOfWork.Roles.AddClaimAsync(existingRole, new Claim("Permission", permission));
            }
        }

        return result;
    }

    public async Task<bool> CanDeleteRoleAsync(string roleId)
    {
        var users = await _unitOfWork.Roles.GetUsersInRoleAsync(roleId);
        return !users.Any();
    }

    public async Task<IList<Claim>> GetRolePermissionsAsync(string roleId)
    {
        var role = await _unitOfWork.Roles.FindByIdAsync(roleId);
        if (role == null)
        {
            return new List<Claim>();
        }

        var claims = await _unitOfWork.Roles.GetClaimsAsync(role);
        return claims.ToList();
    }
}