namespace LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;

/// <summary>
/// Task that waits for an element to reach a specific state.
/// This is useful for explicit waiting when auto-waiting is not sufficient.
/// 
/// Usage:
///   await actor.AttemptsToAsync(WaitForElement.ToBeVisible(successMessage));
///   await actor.AttemptsToAsync(WaitForElement.ToBeHidden(loadingSpinner));
/// </summary>
public class WaitForElement : ITask
{
    private readonly ILocator _locator;
    private readonly WaitForSelectorState _state;
    
    /// <summary>
    /// Private constructor - use factory methods.
    /// </summary>
    private WaitForElement(ILocator locator, WaitForSelectorState state)
    {
        _locator = locator;
        _state = state;
    }
    
    /// <summary>
    /// Creates a WaitForElement task that waits for the element to be visible.
    /// </summary>
    /// <param name="locator">The element to wait for</param>
    /// <returns>A WaitForElement task</returns>
    public static WaitForElement ToBeVisible(ILocator locator) 
        => new(locator, WaitForSelectorState.Visible);
    
    /// <summary>
    /// Creates a WaitForElement task that waits for the element to be hidden.
    /// </summary>
    /// <param name="locator">The element to wait for</param>
    /// <returns>A WaitForElement task</returns>
    public static WaitForElement ToBeHidden(ILocator locator) 
        => new(locator, WaitForSelectorState.Hidden);
    
    /// <summary>
    /// Creates a WaitForElement task that waits for the element to be attached to DOM.
    /// </summary>
    /// <param name="locator">The element to wait for</param>
    /// <returns>A WaitForElement task</returns>
    public static WaitForElement ToExist(ILocator locator) 
        => new(locator, WaitForSelectorState.Attached);
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        await _locator.WaitForAsync(new LocatorWaitForOptions { State = _state });
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => $"Wait for element to be {_state}";
}
