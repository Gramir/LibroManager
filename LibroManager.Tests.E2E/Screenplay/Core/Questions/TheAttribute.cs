namespace LibroManager.Tests.E2E.Screenplay.Core.Questions;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;

/// <summary>
/// Question that retrieves the value of an attribute from an element.
/// This is useful for checking href, src, class, data-* attributes, etc.
/// 
/// Uses a fluent builder pattern for readability:
///   TheAttribute.Of(locator).Named("href")
/// 
/// Usage:
///   string href = await actor.AsksForAsync(TheAttribute.Of(link).Named("href"));
///   string? dataId = await actor.AsksForAsync(TheAttribute.Of(element).Named("data-id"));
/// </summary>
public class TheAttribute : IQuestion<string?>
{
    private readonly ILocator _locator;
    private readonly string _attributeName;
    
    /// <summary>
    /// Private constructor - use the Of().Named() fluent API.
    /// </summary>
    private TheAttribute(ILocator locator, string attributeName)
    {
        _locator = locator;
        _attributeName = attributeName;
    }
    
    /// <summary>
    /// Starts building a TheAttribute question for the specified element.
    /// Call .Named() to specify which attribute to retrieve.
    /// </summary>
    /// <param name="locator">The locator for the element</param>
    /// <returns>An AttributeBuilder to specify the attribute name</returns>
    /// <example>
    /// TheAttribute.Of(loginButton).Named("class")
    /// </example>
    public static AttributeBuilder Of(ILocator locator) => new(locator);
    
    /// <inheritdoc/>
    public async Task<string?> AnsweredByAsync(Actor actor)
    {
        // GetAttributeAsync returns null if the attribute doesn't exist
        return await _locator.GetAttributeAsync(_attributeName);
    }
    
    /// <summary>
    /// Returns a description of this question for logging purposes.
    /// </summary>
    public override string ToString() => $"the '{_attributeName}' attribute of the element";
    
    /// <summary>
    /// Builder class for the fluent TheAttribute.Of().Named() API.
    /// </summary>
    public class AttributeBuilder
    {
        private readonly ILocator _locator;
        
        /// <summary>
        /// Creates a new AttributeBuilder for the given locator.
        /// </summary>
        internal AttributeBuilder(ILocator locator)
        {
            _locator = locator;
        }
        
        /// <summary>
        /// Specifies which attribute to retrieve.
        /// </summary>
        /// <param name="attributeName">The name of the attribute (e.g., "href", "class", "data-id")</param>
        /// <returns>A complete TheAttribute question ready to be asked</returns>
        public TheAttribute Named(string attributeName) => new(_locator, attributeName);
    }
}
