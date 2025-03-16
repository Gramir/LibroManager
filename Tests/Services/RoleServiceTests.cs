using LibroManager.Models;
using LibroManager.Services;
using LibroManager.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using LibroManager.Constants;
using Moq;
using Xunit;

namespace LibroManager.Tests.Services;

public class RoleServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _roleService = new RoleService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetAllRolesAsync_ReturnsAllRoles()
    {
        // Arrange
        var roles = new List<IdentityRole>
        {
            new() { Id = "1", Name = RoleConstants.AdminRole },
            new() { Id = "2", Name = RoleConstants.LibrarianRole }
        };

        _mockUnitOfWork.Setup(u => u.Roles.GetAllRolesAsync())
            .ReturnsAsync(roles);

        // Act
        var result = await _roleService.GetAllRolesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == RoleConstants.AdminRole);
        Assert.Contains(result, r => r.Name == RoleConstants.LibrarianRole);
        _mockUnitOfWork.Verify(u => u.Roles.GetAllRolesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllRolesWithPermissionsAsync_ReturnsRolesWithPermissions()
    {
        // Arrange
        var roles = new List<IdentityRole>
        {
            new() { Id = "1", Name = RoleConstants.AdminRole },
            new() { Id = "2", Name = RoleConstants.LibrarianRole }
        };

        var adminClaims = RoleConstants.DefaultPermissions.AdminPermissions
            .Select(p => new Claim("Permission", p))
            .ToList();

        var librarianClaims = RoleConstants.DefaultPermissions.LibrarianPermissions
            .Select(p => new Claim("Permission", p))
            .ToList();

        _mockUnitOfWork.Setup(u => u.Roles.GetAllRolesAsync())
            .ReturnsAsync(roles);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.AdminRole)))
            .ReturnsAsync(adminClaims);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.LibrarianRole)))
            .ReturnsAsync(librarianClaims);

        // Act
        var result = await _roleService.GetAllRolesWithPermissionsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(RoleConstants.DefaultPermissions.AdminPermissions.Length, result[roles[0].Id].Count);
        Assert.Equal(RoleConstants.DefaultPermissions.LibrarianPermissions.Length, result[roles[1].Id].Count);
    }

    [Fact]
    public async Task GetRolePermissionsAsync_ReturnsPermissionsForRole()
    {
        // Arrange
        var roleId = "1";
        var role = new IdentityRole { Id = roleId, Name = RoleConstants.AdminRole };
        var claims = RoleConstants.DefaultPermissions.AdminPermissions
            .Select(p => new Claim("Permission", p))
            .ToList();

        _mockUnitOfWork.Setup(u => u.Roles.FindByIdAsync(roleId))
            .ReturnsAsync(role);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(role))
            .ReturnsAsync(claims);

        // Act
        var result = await _roleService.GetRolePermissionsAsync(roleId);

        // Assert
        Assert.Equal(RoleConstants.DefaultPermissions.AdminPermissions.Length, result.Count);
        foreach (var permission in RoleConstants.DefaultPermissions.AdminPermissions)
        {
            Assert.Contains(result, c => c.Type == "Permission" && c.Value == permission);
        }
    }

    [Fact]
    public async Task EnsureDefaultRolesExistAsync_CreatesAdminRole_WhenItDoesNotExist()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.Roles.FindByNameAsync(RoleConstants.AdminRole))
            .ReturnsAsync((IdentityRole?)null);

        // Act
        await _roleService.EnsureDefaultRolesExistAsync();

        // Assert
        _mockUnitOfWork.Verify(
            u => u.Roles.CreateAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.AdminRole)), 
            Times.Once);
        foreach (var permission in RoleConstants.DefaultPermissions.AdminPermissions)
        {
            _mockUnitOfWork.Verify(
                u => u.Roles.AddClaimAsync(
                    It.Is<IdentityRole>(r => r.Name == RoleConstants.AdminRole),
                    It.Is<Claim>(c => c.Type == "Permission" && c.Value == permission)
                ),
                Times.Once);
        }
    }

    [Fact]
    public async Task EnsureDefaultRolesExistAsync_CreatesLibrarianRole_WhenItDoesNotExist()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.Roles.FindByNameAsync(RoleConstants.LibrarianRole))
            .ReturnsAsync((IdentityRole?)null);

        // Act
        await _roleService.EnsureDefaultRolesExistAsync();

        // Assert
        _mockUnitOfWork.Verify(
            u => u.Roles.CreateAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.LibrarianRole)), 
            Times.Once);
        foreach (var permission in RoleConstants.DefaultPermissions.LibrarianPermissions)
        {
            _mockUnitOfWork.Verify(
                u => u.Roles.AddClaimAsync(
                    It.Is<IdentityRole>(r => r.Name == RoleConstants.LibrarianRole),
                    It.Is<Claim>(c => c.Type == "Permission" && c.Value == permission)
                ),
                Times.Once);
        }
    }

    [Fact]
    public async Task EnsureDefaultRolesExistAsync_DoesNotCreateRoles_WhenTheyAlreadyExist()
    {
        // Arrange
        var adminRole = new IdentityRole(RoleConstants.AdminRole);
        var librarianRole = new IdentityRole(RoleConstants.LibrarianRole);

        var existingAdminClaims = new List<Claim>();
        var existingLibrarianClaims = new List<Claim>();

        _mockUnitOfWork.Setup(u => u.Roles.FindByNameAsync(RoleConstants.AdminRole))
            .ReturnsAsync(adminRole);
        _mockUnitOfWork.Setup(u => u.Roles.FindByNameAsync(RoleConstants.LibrarianRole))
            .ReturnsAsync(librarianRole);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.AdminRole)))
            .ReturnsAsync(existingAdminClaims);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.LibrarianRole)))
            .ReturnsAsync(existingLibrarianClaims);

        // Act
        await _roleService.EnsureDefaultRolesExistAsync();

        // Assert
        _mockUnitOfWork.Verify(u => u.Roles.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        
        // Verificar que se agreguen los permisos faltantes para Admin
        foreach (var permission in RoleConstants.DefaultPermissions.AdminPermissions)
        {
            _mockUnitOfWork.Verify(
                u => u.Roles.AddClaimAsync(
                    It.Is<IdentityRole>(r => r.Name == RoleConstants.AdminRole),
                    It.Is<Claim>(c => c.Type == "Permission" && c.Value == permission)
                ),
                Times.Once);
        }

        // Verificar que se agreguen los permisos faltantes para Librarian
        foreach (var permission in RoleConstants.DefaultPermissions.LibrarianPermissions)
        {
            _mockUnitOfWork.Verify(
                u => u.Roles.AddClaimAsync(
                    It.Is<IdentityRole>(r => r.Name == RoleConstants.LibrarianRole),
                    It.Is<Claim>(c => c.Type == "Permission" && c.Value == permission)
                ),
                Times.Once);
        }
    }

    [Fact]
    public async Task EnsureDefaultRolesExistAsync_OnlyAddsPermissions_WhenMissing()
    {
        // Arrange
        var adminRole = new IdentityRole(RoleConstants.AdminRole);
        var librarianRole = new IdentityRole(RoleConstants.LibrarianRole);

        // Configurar algunos permisos existentes
        var existingAdminClaims = new List<Claim>
        {
            new Claim("Permission", RoleConstants.DefaultPermissions.AdminPermissions[0])
        };
        var existingLibrarianClaims = new List<Claim>
        {
            new Claim("Permission", RoleConstants.DefaultPermissions.LibrarianPermissions[0])
        };

        _mockUnitOfWork.Setup(u => u.Roles.FindByNameAsync(RoleConstants.AdminRole))
            .ReturnsAsync(adminRole);
        _mockUnitOfWork.Setup(u => u.Roles.FindByNameAsync(RoleConstants.LibrarianRole))
            .ReturnsAsync(librarianRole);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.AdminRole)))
            .ReturnsAsync(existingAdminClaims);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(It.Is<IdentityRole>(r => r.Name == RoleConstants.LibrarianRole)))
            .ReturnsAsync(existingLibrarianClaims);

        // Act
        await _roleService.EnsureDefaultRolesExistAsync();

        // Assert
        _mockUnitOfWork.Verify(u => u.Roles.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        
        // Verificar que solo se agreguen los permisos faltantes para Admin
        foreach (var permission in RoleConstants.DefaultPermissions.AdminPermissions.Skip(1))
        {
            _mockUnitOfWork.Verify(
                u => u.Roles.AddClaimAsync(
                    It.Is<IdentityRole>(r => r.Name == RoleConstants.AdminRole),
                    It.Is<Claim>(c => c.Type == "Permission" && c.Value == permission)
                ),
                Times.Once);
        }
        
        // Verificar que no se agregue el permiso que ya existe para Admin
        _mockUnitOfWork.Verify(
            u => u.Roles.AddClaimAsync(
                It.Is<IdentityRole>(r => r.Name == RoleConstants.AdminRole),
                It.Is<Claim>(c => c.Type == "Permission" && c.Value == RoleConstants.DefaultPermissions.AdminPermissions[0])
            ),
            Times.Never);

        // Verificar que solo se agreguen los permisos faltantes para Librarian
        foreach (var permission in RoleConstants.DefaultPermissions.LibrarianPermissions.Skip(1))
        {
            _mockUnitOfWork.Verify(
                u => u.Roles.AddClaimAsync(
                    It.Is<IdentityRole>(r => r.Name == RoleConstants.LibrarianRole),
                    It.Is<Claim>(c => c.Type == "Permission" && c.Value == permission)
                ),
                Times.Once);
        }

        // Verificar que no se agregue el permiso que ya existe para Librarian
        _mockUnitOfWork.Verify(
            u => u.Roles.AddClaimAsync(
                It.Is<IdentityRole>(r => r.Name == RoleConstants.LibrarianRole),
                It.Is<Claim>(c => c.Type == "Permission" && c.Value == RoleConstants.DefaultPermissions.LibrarianPermissions[0])
            ),
            Times.Never);
    }
}