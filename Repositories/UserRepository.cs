using LibroManager.Data.Context;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibroManager.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
    }

    public async Task<ApplicationUser?> FindByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
    {
        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        return await _userManager.AddToRolesAsync(user, roles);
    }

    public async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        return await _userManager.RemoveFromRolesAsync(user, roles);
    }
}