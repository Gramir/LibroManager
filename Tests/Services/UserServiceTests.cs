using LibroManager.Models;
using LibroManager.DTOs;
using LibroManager.Services;
using LibroManager.Repositories.Interfaces;
using LibroManager.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Moq;
using Xunit;
using AutoMapper;

namespace LibroManager.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<ApplicationUser>
        {
            new() { Id = "1", UserName = "user1@test.com", Email = "user1@test.com", NombreCompleto = "User 1" },
            new() { Id = "2", UserName = "user2@test.com", Email = "user2@test.com", NombreCompleto = "User 2" }
        };

        _mockUnitOfWork.Setup(u => u.Users.GetAllUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _mockUnitOfWork.Verify(u => u.Users.GetAllUsersAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersWithRolesAsync_ReturnsUsersWithRoles()
    {
        // Arrange
        var userId = "1";
        var users = new List<ApplicationUser>
        {
            new() { Id = userId, UserName = "user1@test.com", Email = "user1@test.com", NombreCompleto = "User 1" }
        };
        var roles = new List<string> { "Admin", "User" };

        _mockUnitOfWork.Setup(u => u.Users.GetAllUsersAsync())
            .ReturnsAsync(users);
        _mockUnitOfWork.Setup(u => u.Users.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _userService.GetAllUsersWithRolesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result[userId].Count);
        Assert.Contains("Admin", result[userId]);
        Assert.Contains("User", result[userId]);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsFalse_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "test@test.com",
            Email = "test@test.com",
            NombreCompleto = "Test User"
        };
        var password = "123"; // Contraseña inválida
        var roles = new List<string> { "User" };

        _mockUnitOfWork.Setup(u => u.Users.CreateAsync(user, password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too short" }));

        // Act
        var (result, createdUser) = await _userService.CreateUserAsync(user, password, roles);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(createdUser);
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsTrue_WhenUserIsValid()
    {
        // Arrange
        var user = new ApplicationUser
        {
            UserName = "test@test.com",
            Email = "test@test.com",
            NombreCompleto = "Test User"
        };
        var password = "TestPassword123!";
        var roles = new List<string> { RoleConstants.LibrarianRole };

        _mockUnitOfWork.Setup(u => u.Users.CreateAsync(user, password))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Users.AddToRolesAsync(user, roles))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Users.AddClaimAsync(user, It.Is<Claim>(c => 
            c.Type == "NombreCompleto" && c.Value == user.NombreCompleto)))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var (result, createdUser) = await _userService.CreateUserAsync(user, password, roles);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(createdUser);
        _mockUnitOfWork.Verify(u => u.Users.CreateAsync(user, password), Times.Once);
        _mockUnitOfWork.Verify(u => u.Users.AddToRolesAsync(user, roles), Times.Once);
        _mockUnitOfWork.Verify(u => u.Users.AddClaimAsync(user, 
            It.Is<Claim>(c => c.Type == "NombreCompleto" && c.Value == user.NombreCompleto)), 
            Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsFalse_WhenUserNotFound()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", NombreCompleto = "Test User" };
        var roles = new List<string> { "User" };

        ApplicationUser? nullUser = null;
        _mockUnitOfWork.Setup(u => u.Users.FindByIdAsync(user.Id))
            .ReturnsAsync(nullUser);

        // Act
        var result = await _userService.UpdateUserAsync(user, roles);

        // Assert
        Assert.False(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Users.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserAsync_ReturnsTrue_WhenUserIsValid()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "1",
            UserName = "test@test.com",
            Email = "test@test.com",
            NombreCompleto = "Updated User"
        };
        var roles = new List<string> { RoleConstants.LibrarianRole };
        var currentRoles = new List<string> { RoleConstants.AdminRole };
        var existingClaims = new List<Claim>
        {
            new Claim("NombreCompleto", "Original User")
        };

        _mockUnitOfWork.Setup(u => u.Users.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _mockUnitOfWork.Setup(u => u.Users.GetRolesAsync(user))
            .ReturnsAsync(currentRoles);
        _mockUnitOfWork.Setup(u => u.Users.GetClaimsAsync(user))
            .ReturnsAsync(existingClaims);
        _mockUnitOfWork.Setup(u => u.Users.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Users.RemoveFromRolesAsync(user, currentRoles))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Users.AddToRolesAsync(user, roles))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Users.RemoveClaimAsync(user, It.Is<Claim>(c => 
            c.Type == "NombreCompleto" && c.Value == "Original User")))
            .ReturnsAsync(IdentityResult.Success);
        _mockUnitOfWork.Setup(u => u.Users.AddClaimAsync(user, It.Is<Claim>(c => 
            c.Type == "NombreCompleto" && c.Value == user.NombreCompleto)))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.UpdateUserAsync(user, roles);

        // Assert
        Assert.True(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Users.UpdateAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.Users.RemoveFromRolesAsync(user, currentRoles), Times.Once);
        _mockUnitOfWork.Verify(u => u.Users.AddToRolesAsync(user, roles), Times.Once);
        _mockUnitOfWork.Verify(u => u.Users.RemoveClaimAsync(user, 
            It.Is<Claim>(c => c.Type == "NombreCompleto" && c.Value == "Original User")), 
            Times.Once);
        _mockUnitOfWork.Verify(u => u.Users.AddClaimAsync(user, 
            It.Is<Claim>(c => c.Type == "NombreCompleto" && c.Value == user.NombreCompleto)), 
            Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsFalse_WhenUserNotFound()
    {
        // Arrange
        var userId = "1";

        ApplicationUser? nullUser = null;
        _mockUnitOfWork.Setup(u => u.Users.FindByIdAsync(userId))
            .ReturnsAsync(nullUser);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.False(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Users.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        var userId = "1";
        var user = new ApplicationUser { Id = userId, NombreCompleto = "Test User" };

        _mockUnitOfWork.Setup(u => u.Users.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUnitOfWork.Setup(u => u.Users.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.True(result.Succeeded);
        _mockUnitOfWork.Verify(u => u.Users.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var email = "nonexistent@test.com";

        ApplicationUser? nullUser = null;
        _mockUnitOfWork.Setup(u => u.Users.FindByEmailAsync(email))
            .ReturnsAsync(nullUser);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser_WhenFound()
    {
        // Arrange
        var email = "test@test.com";
        var user = new ApplicationUser { 
            Email = email, 
            NombreCompleto = "Test User" 
        };

        _mockUnitOfWork.Setup(u => u.Users.FindByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }
}