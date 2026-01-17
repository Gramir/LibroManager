namespace LibroManager.Tests.E2E.Screenplay.Core.Abilities;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;

/// <summary>
/// Ability that enables an Actor to browse the web using Playwright.
/// This is the primary ability for web UI automation in the Screenplay pattern.
/// 
/// The BrowseTheWeb ability holds references to:
/// - IPage: The Playwright page object for browser interactions
/// - BaseUrl: The base URL of the application under test
/// 
/// Usage:
///   // Grant the ability to an actor
///   var actor = Actor.Named("Admin")
///       .WhoCan(BrowseTheWeb.Using(page, "http://localhost:5000"));
///   
///   // Retrieve the ability in a Task or Question
///   var browseTheWeb = BrowseTheWeb.As(actor);
///   await browseTheWeb.Page.GotoAsync(browseTheWeb.BaseUrl + "/login");
/// </summary>
public class BrowseTheWeb : IAbility
{
    /// <summary>
    /// The Playwright IPage object used for browser automation.
    /// This is the main entry point for all Playwright interactions.
    /// </summary>
    public IPage Page { get; }
    
    /// <summary>
    /// The base URL of the application under test.
    /// Tasks can use this to construct full URLs for navigation.
    /// The trailing slash is automatically removed for consistency.
    /// </summary>
    public string BaseUrl { get; }
    
    /// <summary>
    /// Private constructor - use the Using() factory method.
    /// </summary>
    /// <param name="page">The Playwright page to use for automation</param>
    /// <param name="baseUrl">The base URL of the application</param>
    private BrowseTheWeb(IPage page, string baseUrl)
    {
        Page = page;
        // Remove trailing slash for consistent URL building
        BaseUrl = baseUrl.TrimEnd('/');
    }
    
    /// <summary>
    /// Creates a new BrowseTheWeb ability with the given Playwright page and base URL.
    /// This is the primary factory method for creating this ability.
    /// </summary>
    /// <param name="page">The Playwright IPage to use for browser interactions</param>
    /// <param name="baseUrl">The base URL of the application under test</param>
    /// <returns>A new BrowseTheWeb ability instance</returns>
    /// <example>
    /// var ability = BrowseTheWeb.Using(page, "http://localhost:5000");
    /// actor.WhoCan(ability);
    /// </example>
    public static BrowseTheWeb Using(IPage page, string baseUrl) 
        => new(page, baseUrl);
    
    /// <summary>
    /// Retrieves the BrowseTheWeb ability from an Actor.
    /// This is a convenience method for use within Tasks and Questions.
    /// Throws MissingAbilityException if the actor doesn't have this ability.
    /// </summary>
    /// <param name="actor">The actor to retrieve the ability from</param>
    /// <returns>The BrowseTheWeb ability</returns>
    /// <exception cref="Exceptions.MissingAbilityException">
    /// If the actor doesn't have the BrowseTheWeb ability
    /// </exception>
    /// <example>
    /// // Inside a Task's PerformAsAsync method:
    /// public async Task PerformAsAsync(Actor actor)
    /// {
    ///     var browseTheWeb = BrowseTheWeb.As(actor);
    ///     await browseTheWeb.Page.GotoAsync(browseTheWeb.BaseUrl + "/login");
    /// }
    /// </example>
    public static BrowseTheWeb As(Actor actor) 
        => actor.GetAbility<BrowseTheWeb>();
    
    /// <summary>
    /// Constructs a full URL by combining the base URL with a relative path.
    /// This is a convenience method for URL construction.
    /// </summary>
    /// <param name="relativePath">The relative path to append to the base URL</param>
    /// <returns>The full URL</returns>
    /// <example>
    /// var fullUrl = browseTheWeb.FullUrl("/Account/Login");
    /// // Returns: "http://localhost:5000/Account/Login"
    /// </example>
    public string FullUrl(string relativePath)
    {
        // Ensure the relative path starts with a slash for consistency
        if (!relativePath.StartsWith('/'))
        {
            relativePath = "/" + relativePath;
        }
        return BaseUrl + relativePath;
    }
}
