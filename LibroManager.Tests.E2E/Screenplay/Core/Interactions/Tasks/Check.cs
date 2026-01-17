namespace LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;

/// <summary>
/// Task that checks (selects) a checkbox or radio button.
/// Uses Playwright's CheckAsync which handles both initial selection and already-selected states.
/// 
/// Usage:
///   await actor.AttemptsToAsync(Check.The(rememberMeCheckbox));
///   await actor.AttemptsToAsync(Check.The(adminRoleRadio));
/// </summary>
public class Check : ITask
{
    private readonly ILocator _locator;
    
    /// <summary>
    /// Private constructor - use The() factory method.
    /// </summary>
    private Check(ILocator locator)
    {
        _locator = locator;
    }
    
    /// <summary>
    /// Creates a Check task for the specified checkbox or radio button.
    /// </summary>
    /// <param name="locator">The locator for the checkbox or radio button</param>
    /// <returns>A Check task</returns>
    public static Check The(ILocator locator) => new(locator);
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        // Playwright's CheckAsync ensures the element is checked
        await _locator.CheckAsync();
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => "Check the element";
}
