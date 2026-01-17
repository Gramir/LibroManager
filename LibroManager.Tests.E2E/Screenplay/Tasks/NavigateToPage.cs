namespace LibroManager.Tests.E2E.Screenplay.Tasks;

using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;
using LibroManager.Tests.E2E.Screenplay.UI;

/// <summary>
/// High-level navigation task that provides semantic navigation to specific pages.
/// This provides more readable test code compared to raw URL paths.
/// 
/// Usage:
///   await actor.AttemptsToAsync(NavigateToPage.Login());
///   await actor.AttemptsToAsync(NavigateToPage.Home());
///   await actor.AttemptsToAsync(NavigateToPage.UserManagement());
/// </summary>
public class NavigateToPage : ITask
{
    private readonly string _pagePath;
    private readonly string _pageName;
    
    /// <summary>
    /// Private constructor - use factory methods.
    /// </summary>
    private NavigateToPage(string pagePath, string pageName)
    {
        _pagePath = pagePath;
        _pageName = pageName;
    }
    
    /// <summary>
    /// Creates a task to navigate to the Login page.
    /// </summary>
    /// <returns>A NavigateToPage task</returns>
    public static NavigateToPage Login() 
        => new(LoginUI.Path, "Login");
    
    /// <summary>
    /// Creates a task to navigate to the Home page.
    /// </summary>
    /// <returns>A NavigateToPage task</returns>
    public static NavigateToPage Home() 
        => new(MainPageUI.Path, "Home");
    
    /// <summary>
    /// Creates a task to navigate to the User Management page.
    /// </summary>
    /// <returns>A NavigateToPage task</returns>
    public static NavigateToPage UserManagement() 
        => new(UserManagementUI.Path, "User Management");
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        await actor.AttemptsToAsync(Navigate.ToPath(_pagePath));
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => $"Navigate to {_pageName} page";
}
