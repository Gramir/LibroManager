namespace LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;

/// <summary>
/// Task that clicks on a web element.
/// This task uses Playwright's auto-waiting - it will wait for the element
/// to be visible and actionable before clicking.
/// 
/// Usage:
///   await actor.AttemptsToAsync(Click.On(loginButton));
///   await actor.AttemptsToAsync(Click.On(page.Locator("button[type='submit']")));
/// </summary>
public class Click : ITask
{
    private readonly ILocator _locator;
    
    /// <summary>
    /// Private constructor - use the On() factory method.
    /// </summary>
    private Click(ILocator locator)
    {
        _locator = locator;
    }
    
    /// <summary>
    /// Creates a Click task for the specified locator.
    /// </summary>
    /// <param name="locator">The Playwright locator identifying the element to click</param>
    /// <returns>A Click task</returns>
    /// <example>
    /// await actor.AttemptsToAsync(Click.On(page.Locator("#submit-button")));
    /// </example>
    public static Click On(ILocator locator) => new(locator);
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        // Playwright's ClickAsync automatically waits for the element to be actionable
        await _locator.ClickAsync();
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => $"Click on element";
}
