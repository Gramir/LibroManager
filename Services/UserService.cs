using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace LibroManager.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
    {
        return await _unitOfWork.Users.GetAllUsersAsync();
    }

    public async Task<Dictionary<string, List<string>>> GetAllUsersWithRolesAsync()
    {
        var users = await GetAllUsersAsync();
        var usersWithRoles = new Dictionary<string, List<string>>();

        foreach (var user in users)
        {
            var roles = await _unitOfWork.Users.GetRolesAsync(user);
            usersWithRoles[user.Id] = roles.ToList();
        }

        return usersWithRoles;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string id)
    {
        return await _unitOfWork.Users.FindByIdAsync(id);
    }

    public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return await _unitOfWork.Users.FindByEmailAsync(email);
    }

    public async Task<(IdentityResult Result, ApplicationUser? User)> CreateUserAsync(ApplicationUser user, string password, IEnumerable<string> roles)
    {
        var result = await _unitOfWork.Users.CreateAsync(user, password);

        if (result.Succeeded)
        {
            if (roles.Any())
            {
                var roleResult = await _unitOfWork.Users.AddToRolesAsync(user, roles);
                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Failed to add roles to user {UserId}: {Errors}", user.Id, string.Join(", ", roleResult.Errors));
                }
            }
            return (result, user);
        }

        return (result, null);
    }

    public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user, IEnumerable<string> roles)
    {
        var existingUser = await _unitOfWork.Users.FindByIdAsync(user.Id);
        if (existingUser == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }

        existingUser.NombreCompleto = user.NombreCompleto;
        var result = await _unitOfWork.Users.UpdateAsync(existingUser);

        if (result.Succeeded)
        {
            var currentRoles = await _unitOfWork.Users.GetRolesAsync(existingUser);
            var rolesToRemove = currentRoles.ToList();
            var rolesToAdd = roles.ToList();

            if (rolesToRemove.Any())
            {
                await _unitOfWork.Users.RemoveFromRolesAsync(existingUser, rolesToRemove);
            }

            if (rolesToAdd.Any())
            {
                await _unitOfWork.Users.AddToRolesAsync(existingUser, rolesToAdd);
            }
        }

        return result;
    }

    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        var user = await _unitOfWork.Users.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });
        }

        return await _unitOfWork.Users.DeleteAsync(user);
    }
}