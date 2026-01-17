namespace LibroManager.Tests.E2E.Screenplay.UI;

using Microsoft.Playwright;

/// <summary>
/// UI Model for the User Management (Admin) page.
/// Contains all locators for elements on the user administration page.
/// 
/// This page is only accessible to admin users and allows creating,
/// editing, and managing user accounts.
/// 
/// Usage:
///   await actor.AttemptsToAsync(Click.On(UserManagementUI.AddUserButton(page)));
/// </summary>
public static class UserManagementUI
{
    /// <summary>
    /// The relative URL path for the user management page.
    /// </summary>
    public const string Path = "/Admin/Usuarios";
    
    /// <summary>
    /// The "New User" button that opens the create user form.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the add user button</returns>
    public static ILocator AddUserButton(IPage page) 
        => page.Locator("button:has-text('Nuevo Usuario')");
    
    /// <summary>
    /// The email input field in the create user form.
    /// Uses ARIA role for reliable identification.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the email input</returns>
    public static ILocator EmailInput(IPage page) 
        => page.GetByRole(AriaRole.Textbox, new() { Name = "Email*" });
    
    /// <summary>
    /// The name input field in the create user form.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the name input</returns>
    public static ILocator NameInput(IPage page) 
        => page.GetByRole(AriaRole.Textbox, new() { Name = "Nombre Completo*" });
    
    /// <summary>
    /// The password input field in the create user form.
    /// Uses Exact matching to distinguish from "Confirm Password".
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the password input</returns>
    public static ILocator PasswordInput(IPage page) 
        => page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña*", Exact = true });
    
    /// <summary>
    /// The confirm password input field in the create user form.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the confirm password input</returns>
    public static ILocator ConfirmPasswordInput(IPage page) 
        => page.GetByRole(AriaRole.Textbox, new() { Name = "Confirmar Contraseña*" });
    
    /// <summary>
    /// The "Bibliotecario" (Librarian) role radio button.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the bibliotecario role radio button</returns>
    public static ILocator RoleBibliotecario(IPage page) 
        => page.GetByRole(AriaRole.Radio, new() { Name = "Bibliotecario" });
    
    /// <summary>
    /// The save/submit button in the create user form.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the save button</returns>
    public static ILocator SaveButton(IPage page) 
        => page.GetByRole(AriaRole.Button, new() { Name = "Guardar" });
    
    /// <summary>
    /// The success message shown after a user is created.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the success message</returns>
    public static ILocator SuccessMessage(IPage page) 
        => page.GetByText("Usuario creado exitosamente");
    
    /// <summary>
    /// Gets a locator for a table row containing a user with the specified email.
    /// This is useful for verifying that a user was created.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <param name="email">The email of the user to find</param>
    /// <returns>A locator for the table row containing the user</returns>
    public static ILocator UserRow(IPage page, string email) 
        => page.Locator($"tr:has(td:has-text('{email}'))");
}
