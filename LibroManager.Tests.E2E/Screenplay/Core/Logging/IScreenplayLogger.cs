namespace LibroManager.Tests.E2E.Screenplay.Core.Logging;

/// <summary>
/// Interface for logging Screenplay actions.
/// Provides visibility into what the Actor is doing during test execution.
/// </summary>
public interface IScreenplayLogger
{
    /// <summary>
    /// Logs when an actor attempts to perform a task.
    /// </summary>
    void LogTask(string actorName, string taskDescription);
    
    /// <summary>
    /// Logs when an actor asks a question.
    /// </summary>
    void LogQuestion(string actorName, string questionDescription);
    
    /// <summary>
    /// Logs the answer to a question.
    /// </summary>
    void LogAnswer<T>(string actorName, string questionDescription, T answer);
    
    /// <summary>
    /// Logs an error that occurred during a Screenplay action.
    /// </summary>
    void LogError(string actorName, string action, Exception exception);
    
    /// <summary>
    /// Logs informational messages.
    /// </summary>
    void LogInfo(string message);
}
