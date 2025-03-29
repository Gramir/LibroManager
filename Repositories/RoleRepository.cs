using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibroManager.Repositories;

public class RoleRepository(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager) : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
    {
        return await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
    }

    public async Task<IdentityRole?> FindByIdAsync(string id)
    {
        return await _roleManager.FindByIdAsync(id);
    }

    public async Task<IdentityRole?> FindByNameAsync(string name)
    {
        return await _roleManager.FindByNameAsync(name);
    }

    public async Task<IdentityResult> CreateAsync(IdentityRole role)
    {
        return await _roleManager.CreateAsync(role);
    }

    public async Task<IdentityResult> UpdateAsync(IdentityRole role)
    {
        return await _roleManager.UpdateAsync(role);
    }

    public async Task<IdentityResult> DeleteAsync(IdentityRole role)
    {
        return await _roleManager.DeleteAsync(role);
    }

    public async Task<IList<Claim>> GetClaimsAsync(IdentityRole role)
    {
        return await _roleManager.GetClaimsAsync(role);
    }

    public async Task<IdentityResult> AddClaimAsync(IdentityRole role, Claim claim)
    {
        return await _roleManager.AddClaimAsync(role, claim);
    }

    public async Task<IdentityResult> RemoveClaimAsync(IdentityRole role, Claim claim)
    {
        return await _roleManager.RemoveClaimAsync(role, claim);
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName)
    {
        return await _userManager.GetUsersInRoleAsync(roleName);
    }
}