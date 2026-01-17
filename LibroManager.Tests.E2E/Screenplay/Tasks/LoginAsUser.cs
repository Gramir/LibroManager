namespace LibroManager.Tests.E2E.Screenplay.Tasks;

using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;
using LibroManager.Tests.E2E.Screenplay.UI;

/// <summary>
/// High-level task that performs a complete login flow.
/// This task composes multiple atomic tasks (Navigate, Fill, Click, Wait)
/// into a single, meaningful action that represents user login.
/// 
/// This demonstrates the Screenplay pattern's composability - complex
/// workflows can be built from simple, reusable building blocks.
/// 
/// Usage:
///   await actor.AttemptsToAsync(LoginAsUser.WithCredentials("user@test.com", "password123"));
///   await actor.AttemptsToAsync(LoginAsUser.AsAdmin());
/// </summary>
public class LoginAsUser : ITask
{
    private readonly string _email;
    private readonly string _password;
    
    /// <summary>
    /// Default admin credentials for testing.
    /// </summary>
    private const string AdminEmail = "admin@libromanager.com";
    private const string AdminPassword = "Admin123!";
    
    /// <summary>
    /// Private constructor - use factory methods.
    /// </summary>
    private LoginAsUser(string email, string password)
    {
        _email = email;
        _password = password;
    }
    
    /// <summary>
    /// Creates a login task with specified credentials.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="password">The user's password</param>
    /// <returns>A LoginAsUser task</returns>
    /// <example>
    /// await actor.AttemptsToAsync(
    ///     LoginAsUser.WithCredentials("test@example.com", "SecurePass123!")
    /// );
    /// </example>
    public static LoginAsUser WithCredentials(string email, string password) 
        => new(email, password);
    
    /// <summary>
    /// Creates a login task using the default admin credentials.
    /// This is a convenience method for tests that need admin access.
    /// </summary>
    /// <returns>A LoginAsUser task with admin credentials</returns>
    /// <example>
    /// await actor.AttemptsToAsync(LoginAsUser.AsAdmin());
    /// </example>
    public static LoginAsUser AsAdmin() 
        => new(AdminEmail, AdminPassword);
    
    /// <summary>
    /// Creates a login task with the specified role-based user.
    /// </summary>
    /// <param name="role">The role name (e.g., "Admin", "Bibliotecario")</param>
    /// <param name="email">The email for the role-based user</param>
    /// <param name="password">The password for the role-based user</param>
    /// <returns>A LoginAsUser task</returns>
    public static LoginAsUser AsRole(string role, string email, string password) 
        => new(email, password);
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        // Get the page from the BrowseTheWeb ability
        var browseTheWeb = BrowseTheWeb.As(actor);
        var page = browseTheWeb.Page;
        
        // Step 1: Navigate to the login page
        await actor.AttemptsToAsync(Navigate.ToLoginPage());
        
        // Step 2: Fill in the email field
        await actor.AttemptsToAsync(Fill.The(LoginUI.EmailInput(page)).With(_email));
        
        // Step 3: Fill in the password field
        await actor.AttemptsToAsync(Fill.The(LoginUI.PasswordInput(page)).With(_password));
        
        // Step 4: Click the login button
        await actor.AttemptsToAsync(Click.On(LoginUI.LoginButton(page)));
        
        // Step 5: Wait for successful navigation to the home page
        await actor.AttemptsToAsync(WaitForUrl.ToBe("/"));
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => $"Login as {_email}";
}
