namespace LibroManager.Tests.E2E.Screenplay.Core.Questions;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;

/// <summary>
/// Question that counts how many elements match a locator.
/// This is useful for verifying lists, tables, or collections of elements.
/// 
/// Usage:
///   int userCount = await actor.AsksForAsync(TheCount.Of(userRows));
///   Assert.Equal(5, await actor.AsksForAsync(TheCount.Of(menuItems)));
/// </summary>
public class TheCount : IQuestion<int>
{
    private readonly ILocator _locator;
    
    /// <summary>
    /// Private constructor - use the Of() factory method.
    /// </summary>
    private TheCount(ILocator locator)
    {
        _locator = locator;
    }
    
    /// <summary>
    /// Creates a TheCount question for the specified locator.
    /// </summary>
    /// <param name="locator">The Playwright locator that may match multiple elements</param>
    /// <returns>A TheCount question</returns>
    /// <example>
    /// int rowCount = await actor.AsksForAsync(TheCount.Of(page.Locator("tr")));
    /// </example>
    public static TheCount Of(ILocator locator) => new(locator);
    
    /// <inheritdoc/>
    public async Task<int> AnsweredByAsync(Actor actor)
    {
        // CountAsync returns the number of elements matching the locator
        return await _locator.CountAsync();
    }
    
    /// <summary>
    /// Returns a description of this question for logging purposes.
    /// </summary>
    public override string ToString() => "the count of matching elements";
}
