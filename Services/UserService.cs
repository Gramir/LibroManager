using AutoMapper;
using LibroManager.Constants;
using LibroManager.Models;
using LibroManager.Repositories.Interfaces;
using LibroManager.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LibroManager.Services;

public class UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserService> _logger = logger;

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
            usersWithRoles[user.Id] = [.. roles];
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
        // Validar que solo se asignen roles permitidos
        if (roles.Any(r => r != RoleConstants.AdminRole && r != RoleConstants.LibrarianRole))
        {
            return (IdentityResult.Failed(new IdentityError { Description = "Solo se permiten los roles: Admin y Librarian" }), null);
        }

        var result = await _unitOfWork.Users.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // Añadir el claim de NombreCompleto
            await _unitOfWork.Users.AddClaimAsync(user, new System.Security.Claims.Claim("NombreCompleto", user.NombreCompleto));

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
        // Validar que solo se asignen roles permitidos
        if (roles.Any(r => r != RoleConstants.AdminRole && r != RoleConstants.LibrarianRole))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Solo se permiten los roles: Admin y Librarian" });
        }

        var existingUser = await _unitOfWork.Users.FindByIdAsync(user.Id);
        if (existingUser == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });
        }

        // Actualizar el nombre completo
        existingUser.NombreCompleto = user.NombreCompleto;
        var result = await _unitOfWork.Users.UpdateAsync(existingUser);

        if (result.Succeeded)
        {
            // Actualizar el claim de NombreCompleto
            var claims = await _unitOfWork.Users.GetClaimsAsync(existingUser);
            var nombreCompletoClaim = claims.FirstOrDefault(c => c.Type == "NombreCompleto");
            if (nombreCompletoClaim != null)
            {
                await _unitOfWork.Users.RemoveClaimAsync(existingUser, nombreCompletoClaim);
            }
            await _unitOfWork.Users.AddClaimAsync(existingUser, new System.Security.Claims.Claim("NombreCompleto", user.NombreCompleto));

            var currentRoles = await _unitOfWork.Users.GetRolesAsync(existingUser);

            // Remover roles actuales
            if (currentRoles.Any())
            {
                await _unitOfWork.Users.RemoveFromRolesAsync(existingUser, currentRoles);
            }

            // Agregar nuevos roles
            if (roles.Any())
            {
                await _unitOfWork.Users.AddToRolesAsync(existingUser, roles);
            }
        }

        return result;
    }

    public async Task<IdentityResult> DeleteUserAsync(string userId)
    {
        var user = await _unitOfWork.Users.FindByIdAsync(userId);
        if (user == null)
        {
            return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });
        }

        return await _unitOfWork.Users.DeleteAsync(user);
    }
}