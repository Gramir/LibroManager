namespace LibroManager.Tests.E2E.Screenplay.UI;

using Microsoft.Playwright;

/// <summary>
/// UI Model for the Main (Home) page.
/// Contains all locators for elements on the main page.
/// 
/// This includes navigation elements and user-related displays.
/// 
/// Usage:
///   bool isVisible = await actor.AsksForAsync(TheVisibility.Of(MainPageUI.UserLogged(page)));
/// </summary>
public static class MainPageUI
{
    /// <summary>
    /// The relative URL path for the main page.
    /// </summary>
    public const string Path = "/";
    
    /// <summary>
    /// The navbar brand/title element.
    /// Usually displays the application name.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the navbar title</returns>
    public static ILocator NavbarTitle(IPage page) 
        => page.Locator("a.navbar-brand");
    
    /// <summary>
    /// The main header (h1) of the page.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the main header</returns>
    public static ILocator MainHeader(IPage page) 
        => page.Locator("h1");
    
    /// <summary>
    /// The "Login" button/link shown when user is not logged in.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the login link</returns>
    public static ILocator LoginLink(IPage page) 
        => page.Locator("a.btn.btn-primary:has-text('Iniciar Sesión')");
    
    /// <summary>
    /// The element that displays the logged-in user's name/email.
    /// Only visible when a user is logged in.
    /// </summary>
    /// <param name="page">The Playwright page object</param>
    /// <returns>A locator for the user name display</returns>
    public static ILocator UserLogged(IPage page) 
        => page.Locator("span.user-name");
}
