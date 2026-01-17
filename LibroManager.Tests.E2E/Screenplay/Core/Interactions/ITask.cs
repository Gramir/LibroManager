namespace LibroManager.Tests.E2E.Screenplay.Core.Interactions;

using LibroManager.Tests.E2E.Screenplay.Core.Actors;

/// <summary>
/// Represents an action that an Actor can perform.
/// Tasks modify the state of the system under test.
/// Examples: Navigate to a page, Click a button, Fill a form field.
/// </summary>
public interface ITask
{
    /// <summary>
    /// Performs this task as the given actor.
    /// </summary>
    /// <param name="actor">The actor performing this task</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task PerformAsAsync(Actor actor);
}
