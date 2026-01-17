namespace LibroManager.Tests.E2E.Screenplay.Core.Questions;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;

/// <summary>
/// Question that retrieves the current value of an input, textarea, or select element.
/// This is the "value" property of form elements, not the displayed text.
/// 
/// Usage:
///   string email = await actor.AsksForAsync(TheValue.Of(emailInput));
///   string selectedOption = await actor.AsksForAsync(TheValue.Of(dropdownSelect));
/// </summary>
public class TheValue : IQuestion<string>
{
    private readonly ILocator _locator;
    
    /// <summary>
    /// Private constructor - use the Of() factory method.
    /// </summary>
    private TheValue(ILocator locator)
    {
        _locator = locator;
    }
    
    /// <summary>
    /// Creates a TheValue question for the specified form element.
    /// </summary>
    /// <param name="locator">The Playwright locator for an input, textarea, or select</param>
    /// <returns>A TheValue question</returns>
    /// <example>
    /// string inputValue = await actor.AsksForAsync(TheValue.Of(searchInput));
    /// </example>
    public static TheValue Of(ILocator locator) => new(locator);
    
    /// <inheritdoc/>
    public async Task<string> AnsweredByAsync(Actor actor)
    {
        // InputValueAsync gets the value property of input/textarea/select elements
        return await _locator.InputValueAsync();
    }
    
    /// <summary>
    /// Returns a description of this question for logging purposes.
    /// </summary>
    public override string ToString() => "the value of the input element";
}
