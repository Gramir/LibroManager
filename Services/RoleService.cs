using LibroManager.Constants;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

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

    public async Task<IList<Claim>> GetRolePermissionsAsync(string roleId)
    {
        var role = await _unitOfWork.Roles.FindByIdAsync(roleId);
        if (role == null) return new List<Claim>();
        return await _unitOfWork.Roles.GetClaimsAsync(role);
    }

    public async Task EnsureDefaultRolesExistAsync()
    {
        // Asegurar rol Admin
        var adminRole = await _unitOfWork.Roles.FindByNameAsync(RoleConstants.AdminRole);
        if (adminRole == null)
        {
            adminRole = new IdentityRole(RoleConstants.AdminRole);
            await _unitOfWork.Roles.CreateAsync(adminRole);
        }

        // Verificar y actualizar permisos del Admin
        var adminClaims = (await _unitOfWork.Roles.GetClaimsAsync(adminRole))?.Select(c => c.Value).ToList() ?? new List<string>();
        var adminPermissionsToAdd = RoleConstants.DefaultPermissions.AdminPermissions
            .Except(adminClaims);

        foreach (var permission in adminPermissionsToAdd)
        {
            await _unitOfWork.Roles.AddClaimAsync(adminRole, new Claim("Permission", permission));
        }

        // Asegurar rol Librarian
        var librarianRole = await _unitOfWork.Roles.FindByNameAsync(RoleConstants.LibrarianRole);
        if (librarianRole == null)
        {
            librarianRole = new IdentityRole(RoleConstants.LibrarianRole);
            await _unitOfWork.Roles.CreateAsync(librarianRole);
        }

        // Verificar y actualizar permisos del Librarian
        var librarianClaims = (await _unitOfWork.Roles.GetClaimsAsync(librarianRole))?.Select(c => c.Value).ToList() ?? new List<string>();
        var librarianPermissionsToAdd = RoleConstants.DefaultPermissions.LibrarianPermissions
            .Except(librarianClaims);

        foreach (var permission in librarianPermissionsToAdd)
        {
            await _unitOfWork.Roles.AddClaimAsync(librarianRole, new Claim("Permission", permission));
        }
    }
}