using LibroManager.Models;
using LibroManager.Services;
using LibroManager.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
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
            new() { Id = "1", Name = "Admin" },
            new() { Id = "2", Name = "User" }
        };

        _mockUnitOfWork.Setup(u => u.Roles.GetAllRolesAsync())
            .ReturnsAsync(roles);

        // Act
        var result = await _roleService.GetAllRolesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _mockUnitOfWork.Verify(u => u.Roles.GetAllRolesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllRolesWithPermissionsAsync_ReturnsRolesWithPermissions()
    {
        // Arrange
        var roleId = "1";
        var roles = new List<IdentityRole>
        {
            new() { Id = roleId, Name = "Admin" }
        };
        var claims = new List<Claim>
        {
            new("Permission", "Users.Create"),
            new("Permission", "Users.Read")
        };

        _mockUnitOfWork.Setup(u => u.Roles.GetAllRolesAsync())
            .ReturnsAsync(roles);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(claims);

        // Act
        var result = await _roleService.GetAllRolesWithPermissionsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result[roleId].Count);
        Assert.Contains("Users.Create", result[roleId]);
        Assert.Contains("Users.Read", result[roleId]);
    }

    [Fact]
    public async Task CreateRoleAsync_ReturnsFalse_WhenRoleNameIsEmpty()
    {
        // Arrange
        var roleName = "";
        var permissions = new List<string> { "Users.Create" };

        // Act
        var result = await _roleService.CreateRoleAsync(roleName, permissions);

        // Assert
        Assert.False(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Roles.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
    }

    [Fact]
    public async Task CreateRoleAsync_ReturnsTrue_WhenRoleIsValid()
    {
        // Arrange
        var roleName = "TestRole";
        var permissions = new List<string> { "Users.Create", "Users.Read" };

        _mockUnitOfWork.Setup(u => u.Roles.CreateAsync(It.IsAny<IdentityRole>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Roles.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _roleService.CreateRoleAsync(roleName, permissions);

        // Assert
        Assert.True(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Roles.CreateAsync(It.Is<IdentityRole>(r => r.Name == roleName)), Times.Once);
        _mockUnitOfWork.Verify(u => u.Roles.AddClaimAsync(It.IsAny<IdentityRole>(), It.IsAny<Claim>()), Times.Exactly(2));
    }

    [Fact]
    public async Task UpdateRoleAsync_ReturnsFalse_WhenRoleNotFound()
    {
        // Arrange
        var role = new IdentityRole { Id = "1" };
        var permissions = new List<string> { "Users.Create" };

        IdentityRole? nullRole = null;
        _mockUnitOfWork.Setup(u => u.Roles.FindByIdAsync(role.Id))
            .ReturnsAsync(nullRole);

        // Act
        var result = await _roleService.UpdateRoleAsync(role, permissions);

        // Assert
        Assert.False(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Roles.UpdateAsync(It.IsAny<IdentityRole>()), Times.Never);
    }

    [Fact]
    public async Task UpdateRoleAsync_ReturnsTrue_WhenRoleIsValid()
    {
        // Arrange
        var role = new IdentityRole
        {
            Id = "1",
            Name = "TestRole"
        };
        var permissions = new List<string> { "Users.Create", "Users.Read" };
        var currentClaims = new List<Claim>
        {
            new("Permission", "Users.Delete")
        };

        _mockUnitOfWork.Setup(u => u.Roles.FindByIdAsync(role.Id))
            .ReturnsAsync(role);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(role))
            .ReturnsAsync(currentClaims);
        _mockUnitOfWork.Setup(u => u.Roles.UpdateAsync(role))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Roles.RemoveClaimAsync(role, It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Roles.AddClaimAsync(role, It.IsAny<Claim>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _roleService.UpdateRoleAsync(role, permissions);

        // Assert
        Assert.True(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Roles.UpdateAsync(role), Times.Once);
        _mockUnitOfWork.Verify(u => u.Roles.RemoveClaimAsync(role, It.IsAny<Claim>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.Roles.AddClaimAsync(role, It.IsAny<Claim>()), Times.Exactly(2));
    }

    [Fact]
    public async Task DeleteRoleAsync_ReturnsFalse_WhenRoleNotFound()
    {
        // Arrange
        var roleId = "1";

        IdentityRole? nullRole = null;
        _mockUnitOfWork.Setup(u => u.Roles.FindByIdAsync(roleId))
            .ReturnsAsync(nullRole);

        // Act
        var result = await _roleService.DeleteRoleAsync(roleId);

        // Assert
        Assert.False(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Roles.DeleteAsync(It.IsAny<IdentityRole>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRoleAsync_ReturnsTrue_WhenRoleExists()
    {
        // Arrange
        var roleId = "1";
        var role = new IdentityRole { Id = roleId };

        _mockUnitOfWork.Setup(u => u.Roles.FindByIdAsync(roleId))
            .ReturnsAsync(role);
        _mockUnitOfWork.Setup(u => u.Roles.DeleteAsync(role))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _roleService.DeleteRoleAsync(roleId);

        // Assert
        Assert.True(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Roles.DeleteAsync(role), Times.Once);
    }

    [Fact]
    public async Task CanDeleteRoleAsync_ReturnsFalse_WhenRoleHasUsers()
    {
        // Arrange
        var roleId = "1";
        var users = new List<ApplicationUser> { new() { Id = "1", NombreCompleto = "Test User" } };

        _mockUnitOfWork.Setup(u => u.Roles.GetUsersInRoleAsync(roleId))
            .ReturnsAsync(users);

        // Act
        var result = await _roleService.CanDeleteRoleAsync(roleId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CanDeleteRoleAsync_ReturnsTrue_WhenRoleHasNoUsers()
    {
        // Arrange
        var roleId = "1";
        var users = new List<ApplicationUser>();

        _mockUnitOfWork.Setup(u => u.Roles.GetUsersInRoleAsync(roleId))
            .ReturnsAsync(users);

        // Act
        var result = await _roleService.CanDeleteRoleAsync(roleId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetRolePermissionsAsync_ReturnsPermissionsForRole()
    {
        // Arrange
        var roleId = "1";
        var role = new IdentityRole { Id = roleId, Name = "TestRole" };
        var claims = new List<Claim>
        {
            new("Permission", "Users.Create"),
            new("Permission", "Users.Read"),
            new("Other", "OtherClaim")
        };

        _mockUnitOfWork.Setup(u => u.Roles.FindByIdAsync(roleId))
            .ReturnsAsync(role);
        _mockUnitOfWork.Setup(u => u.Roles.GetClaimsAsync(role))
            .ReturnsAsync(claims);

        // Act
        var result = await _roleService.GetRolePermissionsAsync(roleId);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(result, c => c.Value == "Users.Create" && c.Type == "Permission");
        Assert.Contains(result, c => c.Value == "Users.Read" && c.Type == "Permission");
        Assert.Contains(result, c => c.Value == "OtherClaim" && c.Type == "Other");
    }
}