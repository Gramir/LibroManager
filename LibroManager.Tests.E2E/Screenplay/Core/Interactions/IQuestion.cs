namespace LibroManager.Tests.E2E.Screenplay.Core.Interactions;

using LibroManager.Tests.E2E.Screenplay.Core.Actors;

/// <summary>
/// Represents a query that an Actor can ask about the system under test.
/// Questions return a value without modifying state (read-only operations).
/// Examples: Get text of an element, Check visibility, Get attribute value.
/// </summary>
/// <typeparam name="TAnswer">The type of the answer to this question</typeparam>
public interface IQuestion<TAnswer>
{
    /// <summary>
    /// Answers this question for the given actor.
    /// </summary>
    /// <param name="actor">The actor asking this question</param>
    /// <returns>The answer to the question</returns>
    Task<TAnswer> AnsweredByAsync(Actor actor);
}
