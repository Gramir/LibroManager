using LibroManager.Constants;
using LibroManager.Models;
using LibroManager.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LibroManager.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    AuthenticationStateProvider authenticationStateProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

    public async Task<(bool Success, string[] Errors)> LoginAsync(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, new[] { "Usuario no encontrado." });
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            // Añadir el claim de NombreCompleto
            var claims = await _userManager.GetClaimsAsync(user);
            if (!claims.Any(c => c.Type == "NombreCompleto"))
            {
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("NombreCompleto", user.NombreCompleto));
                await _signInManager.RefreshSignInAsync(user);
            }
            return (true, Array.Empty<string>());
        }

        if (result.IsLockedOut)
        {
            return (false, new[] { "La cuenta está bloqueada. Por favor, intente más tarde." });
        }

        return (false, new[] { "Credenciales inválidas." });
    }

    public async Task<(bool Success, string[] Errors)> RegisterAsync(string email, string password, string nombreCompleto, string role)
    {
        // Validar que el rol sea uno de los dos permitidos
        if (role != RoleConstants.AdminRole && role != RoleConstants.LibrarianRole)
        {
            return (false, new[] { "Rol no válido. Solo se permiten los roles: Admin y Librarian." });
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            NombreCompleto = nombreCompleto,
            FechaCreacion = DateTime.UtcNow,
            EmailConfirmed = true // Para simplificar, confirmamos el email automáticamente
        };

        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            // Añadir el claim de NombreCompleto
            await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("NombreCompleto", nombreCompleto));

            if (await _roleManager.RoleExistsAsync(role))
            {
                await _userManager.AddToRoleAsync(user, role);
                return (true, Array.Empty<string>());
            }
            return (false, new[] { "El rol especificado no existe." });
        }

        return (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated ?? false)
        {
            return await _userManager.FindByEmailAsync(user.Identity.Name!);
        }

        return null;
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
    {
        // Validar que el rol sea uno de los dos permitidos
        if (role != RoleConstants.AdminRole && role != RoleConstants.LibrarianRole)
        {
            return false;
        }
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<(bool Success, string[] Errors)> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
    {
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
        {
            return (true, Array.Empty<string>());
        }

        return (false, result.Errors.Select(e => e.Description).ToArray());
    }
}