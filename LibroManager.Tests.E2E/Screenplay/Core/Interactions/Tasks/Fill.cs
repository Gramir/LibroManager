namespace LibroManager.Tests.E2E.Screenplay.Core.Interactions.Tasks;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;

/// <summary>
/// Task that fills an input field with text.
/// Uses Playwright's FillAsync which clears the field first and then types the value.
/// 
/// This task uses a fluent builder pattern for readability:
///   Fill.The(locator).With("value")
/// 
/// Usage:
///   await actor.AttemptsToAsync(Fill.The(emailInput).With("user@example.com"));
///   await actor.AttemptsToAsync(Fill.The(passwordInput).With("secret123"));
/// </summary>
public class Fill : ITask
{
    private readonly ILocator _locator;
    private readonly string _value;
    
    /// <summary>
    /// Private constructor - use The().With() fluent API.
    /// </summary>
    private Fill(ILocator locator, string value)
    {
        _locator = locator;
        _value = value;
    }
    
    /// <summary>
    /// Starts building a Fill task for the specified input locator.
    /// Call .With() to complete the task configuration.
    /// </summary>
    /// <param name="locator">The locator for the input field to fill</param>
    /// <returns>A FillBuilder to specify the value</returns>
    /// <example>
    /// Fill.The(emailInput).With("test@example.com")
    /// </example>
    public static FillBuilder The(ILocator locator) => new(locator);
    
    /// <inheritdoc/>
    public async Task PerformAsAsync(Actor actor)
    {
        // Playwright's FillAsync clears the field and types the new value
        await _locator.FillAsync(_value);
    }
    
    /// <summary>
    /// Returns a description of this task for logging purposes.
    /// </summary>
    public override string ToString() => $"Fill input with '{_value}'";
    
    /// <summary>
    /// Builder class for the fluent Fill.The().With() API.
    /// This provides a more readable way to specify both the locator and value.
    /// </summary>
    public class FillBuilder
    {
        private readonly ILocator _locator;
        
        /// <summary>
        /// Creates a new FillBuilder for the given locator.
        /// </summary>
        internal FillBuilder(ILocator locator)
        {
            _locator = locator;
        }
        
        /// <summary>
        /// Completes the Fill task with the specified value.
        /// </summary>
        /// <param name="value">The text value to fill into the input</param>
        /// <returns>A complete Fill task ready to be performed</returns>
        public Fill With(string value) => new(_locator, value);
    }
}
