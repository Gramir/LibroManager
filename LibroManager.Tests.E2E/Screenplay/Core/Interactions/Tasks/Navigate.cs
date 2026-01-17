namespace LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;

using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;

/// <summary>
/// Task that navigates the browser to a specified URL.
/// This is a fundamental task for web automation - it loads a page at a given address.
/// 
/// The Navigate task supports two modes:
/// 1. Navigate to a relative path (combined with the actor's base URL)
/// 2. Navigate to an absolute URL
/// 
/// Usage:
///   await actor.AttemptsToAsync(Navigate.ToPath("/Account/Login"));
///   await actor.AttemptsToAsync(Navigate.ToHomePage());
///   await actor.AttemptsToAsync(Navigate.ToUrl("https://example.com"));
/// </summary>
public class Navigate : ITask
{
    private readonly string _path;
    private readonly bool _isAbsoluteUrl;
    
    /// <summary>
    /// Private constructor - use factory methods.
    /// </summary>
    private Navigate(string path, bool isAbsoluteUrl = false)
    {
        _path = path;
        _isAbsoluteUrl = isAbsoluteUrl;
    }
    
    /// <summary>
    /// Creates a Navigate task to go to a relative path.
    /// The path will be combined with the actor's base URL.
    /// </summary>
    /// <param name="relativePath">The relative path (e.g., "/Account/Login")</param>
    /// <returns>A Navigate task</returns>
    public static Navigate ToPath(string relativePath) => new(relativePath, false);
    
    /// <summary>
    /// Creates a Navigate task to go to the home page ("/").
    /// </summary>
    /// <returns>A Navigate task to the home page</returns>
    public static Navigate ToHomePage() => new("/", false);
    
    /// <summary>
    /// Creates a Navigate task to go to the login page.
    /// </summary>
    /// <returns>A Navigate task to /Account/Login</returns>
    public static Navigate ToLoginPage() => new("/Account/Login", false);
    
    /// <summary>
    /// Creates a Navigate task to go to an absolute URL.
    /// Use this when navigating outside the application under test.
    /// </summary>
    /// <param name="absoluteUrl">The full URL to navigate to</param>
    /// <returns>A Navigate task</returns>
    public static Navigate ToUrl(string absoluteUrl) => new(absoluteUrl, true);
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        // Get the BrowseTheWeb ability from the actor
        var browseTheWeb = BrowseTheWeb.As(actor);
        
        // Determine the full URL to navigate to
        var url = _isAbsoluteUrl ? _path : browseTheWeb.FullUrl(_path);
        
        // Perform the navigation using Playwright
        await browseTheWeb.Page.GotoAsync(url);
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => $"Navigate to {_path}";
}
