namespace LibroManager.Tests.E2E.Screenplay.UI;

using Microsoft.Playwright;

/// <summary>
/// UI Model for the Login page.
/// Contains all locators for elements on the login page.
/// 
/// In the Screenplay pattern, UI models are static classes that provide
/// locators without containing any interaction logic. This separates
/// the "what" (elements) from the "how" (interactions).
/// 
/// Usage:
///   await actor.AttemptsToAsync(Fill.The(LoginUI.EmailInput(page)).With("user@test.com"));
/// </summary>
public static class LoginUI
{
    /// <summary>
    /// The relative URL path for the login page.
    /// </summary>
    public const string Path = "/Account/Login";
    
    /// <summary>
    /// The email input field on the login form.
    /// Uses the autocomplete attribute for reliable identification.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the email input</returns>
    public static ILocator EmailInput(IPage page) 
        => page.Locator("input[autocomplete='username']");
    
    /// <summary>
    /// The password input field on the login form.
    /// Uses the autocomplete attribute for reliable identification.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the password input</returns>
    public static ILocator PasswordInput(IPage page) 
        => page.Locator("input[autocomplete='current-password']");
    
    /// <summary>
    /// The login submit button.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the login button</returns>
    public static ILocator LoginButton(IPage page) 
        => page.Locator("button[type='submit']");
    
    /// <summary>
    /// The error message container shown when login fails.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the error message</returns>
    public static ILocator ErrorMessage(IPage page) 
        => page.Locator(".alert-danger");
    
    /// <summary>
    /// The login box container (used for visual regression testing).
    /// Contains the entire login form area.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the login box</returns>
    public static ILocator LoginBox(IPage page) 
        => page.GetByText("Iniciar Sesión Correo Electró");
}
