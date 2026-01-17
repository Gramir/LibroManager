namespace LibroManager.Tests.E2E.Screenplay.Core.Questions;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;

/// <summary>
/// Question that retrieves the inner text content of an element.
/// This is one of the most commonly used Questions for verifying displayed text.
/// 
/// The text returned is the visible text content, with whitespace normalized.
/// 
/// Usage:
///   string userName = await actor.AsksForAsync(TheText.Of(userNameLabel));
///   string title = await actor.AsksForAsync(TheText.Of(page.Locator("h1")));
/// </summary>
public class TheText : IQuestion<string>
{
    private readonly ILocator _locator;
    
    /// <summary>
    /// Private constructor - use the Of() factory method.
    /// </summary>
    private TheText(ILocator locator)
    {
        _locator = locator;
    }
    
    /// <summary>
    /// Creates a TheText question for the specified element.
    /// </summary>
    /// <param name="locator">The Playwright locator for the element</param>
    /// <returns>A TheText question</returns>
    /// <example>
    /// var text = await actor.AsksForAsync(TheText.Of(page.Locator(".message")));
    /// </example>
    public static TheText Of(ILocator locator) => new(locator);
    
    /// <inheritdoc/>
    public async Task<string> AnsweredByAsync(Actor actor)
    {
        // InnerTextAsync returns the visible text content of the element
        return await _locator.InnerTextAsync();
    }
    
    /// <summary>
    /// Returns a description of this question for logging purposes.
    /// </summary>
    public override string ToString() => "the text of the element";
}
