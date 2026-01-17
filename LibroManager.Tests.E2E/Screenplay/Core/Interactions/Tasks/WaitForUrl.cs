namespace LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;

using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;

/// <summary>
/// Task that waits for the browser URL to match a specified pattern.
/// This is useful for waiting after navigation or form submissions.
/// 
/// The task supports:
/// - Exact URL matching (with base URL)
/// - Pattern matching using glob patterns
/// 
/// Usage:
///   await actor.AttemptsToAsync(WaitForUrl.ToBe("/"));
///   await actor.AttemptsToAsync(WaitForUrl.ToContain("Account/Login"));
/// </summary>
public class WaitForUrl : ITask
{
    private readonly string _urlPattern;
    private readonly bool _isExact;
    
    /// <summary>
    /// Private constructor - use factory methods.
    /// </summary>
    private WaitForUrl(string urlPattern, bool isExact)
    {
        _urlPattern = urlPattern;
        _isExact = isExact;
    }
    
    /// <summary>
    /// Creates a WaitForUrl task that waits for an exact URL match.
    /// The path is combined with the actor's base URL.
    /// </summary>
    /// <param name="relativePath">The expected relative path (e.g., "/" for home page)</param>
    /// <returns>A WaitForUrl task</returns>
    public static WaitForUrl ToBe(string relativePath) => new(relativePath, true);
    
    /// <summary>
    /// Creates a WaitForUrl task that waits for the URL to contain a pattern.
    /// Uses Playwright's glob pattern matching.
    /// </summary>
    /// <param name="pattern">The pattern the URL should contain</param>
    /// <returns>A WaitForUrl task</returns>
    public static WaitForUrl ToContain(string pattern) => new(pattern, false);
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        var browseTheWeb = BrowseTheWeb.As(actor);
        
        if (_isExact)
        {
            // Wait for exact URL match
            var expectedUrl = browseTheWeb.FullUrl(_urlPattern);
            await browseTheWeb.Page.WaitForURLAsync(expectedUrl);
        }
        else
        {
            // Wait for URL to contain the pattern (using glob)
            await browseTheWeb.Page.WaitForURLAsync($"**{_urlPattern}**");
        }
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => 
        _isExact ? $"Wait for URL to be '{_urlPattern}'" : $"Wait for URL to contain '{_urlPattern}'";
}
