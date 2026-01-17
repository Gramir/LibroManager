namespace LibroManager.Tests.E2E.Screenplay.Core.Questions;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;

/// <summary>
/// Question that checks whether an element is visible on the page.
/// An element is considered visible if it has non-zero dimensions and is not hidden by CSS.
/// 
/// This is useful for asserting that elements are displayed or hidden based on application state.
/// 
/// Usage:
///   bool isVisible = await actor.AsksForAsync(TheVisibility.Of(errorMessage));
///   Assert.True(await actor.AsksForAsync(TheVisibility.Of(successBanner)));
/// </summary>
public class TheVisibility : IQuestion<bool>
{
    private readonly ILocator _locator;
    
    /// <summary>
    /// Private constructor - use the Of() factory method.
    /// </summary>
    private TheVisibility(ILocator locator)
    {
        _locator = locator;
    }
    
    /// <summary>
    /// Creates a TheVisibility question for the specified element.
    /// </summary>
    /// <param name="locator">The Playwright locator for the element</param>
    /// <returns>A TheVisibility question</returns>
    /// <example>
    /// bool isErrorShown = await actor.AsksForAsync(TheVisibility.Of(errorMessage));
    /// </example>
    public static TheVisibility Of(ILocator locator) => new(locator);
    
    /// <inheritdoc/>
    public async Task<bool> AnsweredByAsync(Actor actor)
    {
        // IsVisibleAsync checks if the element is visible (not hidden by CSS, has dimensions)
        return await _locator.IsVisibleAsync();
    }
    
    /// <summary>
    /// Returns a description of this question for logging purposes.
    /// </summary>
    public override string ToString() => "the visibility of the element";
}
