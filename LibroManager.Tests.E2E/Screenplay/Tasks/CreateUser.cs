namespace LibroManager.Tests.E2E.Screenplay.Tasks;

using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;
using LibroManager.Tests.E2E.Screenplay.UI;

/// <summary>
/// High-level task that creates a new user through the admin interface.
/// This task navigates to the user management page, opens the create form,
/// fills in all required fields, and submits.
/// 
/// Requires the actor to be logged in as an admin.
/// 
/// Usage:
///   await actor.AttemptsToAsync(
///       CreateUser.WithDetails("Test User", "test@test.com", "Pass123!")
///           .AsRole("Bibliotecario")
///   );
/// </summary>
public class CreateUser : ITask
{
    private readonly string _name;
    private readonly string _email;
    private readonly string _password;
    private readonly string _role;
    
    /// <summary>
    /// Private constructor - use factory methods.
    /// </summary>
    private CreateUser(string name, string email, string password, string role)
    {
        _name = name;
        _email = email;
        _password = password;
        _role = role;
    }
    
    /// <summary>
    /// Starts building a CreateUser task with the specified details.
    /// Call .AsRole() to specify the user's role.
    /// </summary>
    /// <param name="name">The user's full name</param>
    /// <param name="email">The user's email address</param>
    /// <param name="password">The user's password</param>
    /// <returns>A CreateUserBuilder to specify the role</returns>
    /// <example>
    /// CreateUser.WithDetails("John Doe", "john@test.com", "SecurePass!")
    ///     .AsRole("Bibliotecario")
    /// </example>
    public static CreateUserBuilder WithDetails(string name, string email, string password) 
        => new(name, email, password);
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        var browseTheWeb = BrowseTheWeb.As(actor);
        var page = browseTheWeb.Page;
        
        // Step 1: Navigate to the user management page
        await actor.AttemptsToAsync(Navigate.ToPath(UserManagementUI.Path));
        
        // Step 2: Click the "New User" button to open the form
        await actor.AttemptsToAsync(Click.On(UserManagementUI.AddUserButton(page)));
        
        // Step 3: Fill in the user details
        await actor.AttemptsToAsync(
            Fill.The(UserManagementUI.EmailInput(page)).With(_email)
        );
        await actor.AttemptsToAsync(
            Fill.The(UserManagementUI.NameInput(page)).With(_name)
        );
        await actor.AttemptsToAsync(
            Fill.The(UserManagementUI.PasswordInput(page)).With(_password)
        );
        await actor.AttemptsToAsync(
            Fill.The(UserManagementUI.ConfirmPasswordInput(page)).With(_password)
        );
        
        // Step 4: Select the role (currently only Bibliotecario is implemented)
        if (_role.Contains("Biblio", StringComparison.OrdinalIgnoreCase))
        {
            await actor.AttemptsToAsync(Check.The(UserManagementUI.RoleBibliotecario(page)));
        }
        
        // Step 5: Click the save button
        await actor.AttemptsToAsync(Click.On(UserManagementUI.SaveButton(page)));
        
        // Step 6: Wait for the success message
        await actor.AttemptsToAsync(
            WaitForElement.ToBeVisible(UserManagementUI.SuccessMessage(page))
        );
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => $"Create user {_email} with role {_role}";
    
    /// <summary>
    /// Builder class for the fluent CreateUser API.
    /// </summary>
    public class CreateUserBuilder
    {
        private readonly string _name;
        private readonly string _email;
        private readonly string _password;
        
        /// <summary>
        /// Creates a new builder with the specified user details.
        /// </summary>
        internal CreateUserBuilder(string name, string email, string password)
        {
            _name = name;
            _email = email;
            _password = password;
        }
        
        /// <summary>
        /// Specifies the role for the new user.
        /// </summary>
        /// <param name="role">The role name (e.g., "Bibliotecario", "Admin")</param>
        /// <returns>A complete CreateUser task</returns>
        public CreateUser AsRole(string role) 
            => new(_name, _email, _password, role);
        
        /// <summary>
        /// Creates the user as a Bibliotecario (Librarian).
        /// </summary>
        /// <returns>A complete CreateUser task with Bibliotecario role</returns>
        public CreateUser AsBibliotecario() 
            => new(_name, _email, _password, "Bibliotecario");
    }
}
