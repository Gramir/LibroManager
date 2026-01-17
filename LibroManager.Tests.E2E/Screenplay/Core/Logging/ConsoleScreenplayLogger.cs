namespace LibroManager.Tests.E2E.Screenplay.Core.Logging;

using System;

/// <summary>
/// Default implementation of IScreenplayLogger that writes to the console.
/// Provides clear, formatted output for debugging and test visibility.
/// </summary>
public class ConsoleScreenplayLogger : IScreenplayLogger
{
    private readonly string _prefix;
    
    /// <summary>
    /// Creates a new ConsoleScreenplayLogger with optional prefix.
    /// </summary>
    /// <param name="prefix">Optional prefix for all log messages</param>
    public ConsoleScreenplayLogger(string prefix = "[Screenplay]")
    {
        _prefix = prefix;
    }
    
    /// <inheritdoc/>
    public void LogTask(string actorName, string taskDescription)
    {
        Console.WriteLine($"{_prefix} {actorName} attempts to {taskDescription}");
    }
    
    /// <inheritdoc/>
    public void LogQuestion(string actorName, string questionDescription)
    {
        Console.WriteLine($"{_prefix} {actorName} asks for {questionDescription}");
    }
    
    /// <inheritdoc/>
    public void LogAnswer<T>(string actorName, string questionDescription, T answer)
    {
        Console.WriteLine($"{_prefix} {actorName} sees {questionDescription} = {answer}");
    }
    
    /// <inheritdoc/>
    public void LogError(string actorName, string action, Exception exception)
    {
        Console.WriteLine($"{_prefix} ERROR: {actorName} failed to {action}");
        Console.WriteLine($"{_prefix}   Exception: {exception.Message}");
    }
    
    /// <inheritdoc/>
    public void LogInfo(string message)
    {
        Console.WriteLine($"{_prefix} {message}");
    }
}
