namespace LibroManager.Tests.E2E.Screenplay.Core.Exceptions;

using System;

/// <summary>
/// Base exception for all Screenplay-related errors.
/// Provides descriptive error messages for better debugging.
/// </summary>
public class ScreenplayException : Exception
{
    /// <summary>
    /// The name of the actor that encountered the error.
    /// </summary>
    public string ActorName { get; }
    
    /// <summary>
    /// Creates a new ScreenplayException.
    /// </summary>
    public ScreenplayException(string actorName, string message) 
        : base($"Actor '{actorName}': {message}")
    {
        ActorName = actorName;
    }
    
    /// <summary>
    /// Creates a new ScreenplayException with an inner exception.
    /// </summary>
    public ScreenplayException(string actorName, string message, Exception innerException)
        : base($"Actor '{actorName}': {message}", innerException)
    {
        ActorName = actorName;
    }
}

/// <summary>
/// Thrown when an Actor tries to use an Ability they don't have.
/// </summary>
public class MissingAbilityException : ScreenplayException
{
    /// <summary>
    /// The type of the missing ability.
    /// </summary>
    public Type AbilityType { get; }
    
    /// <summary>
    /// Creates a new MissingAbilityException.
    /// </summary>
    public MissingAbilityException(string actorName, Type abilityType)
        : base(actorName, $"does not have the ability '{abilityType.Name}'. " +
                          $"Make sure to grant this ability using Actor.WhoCan()")
    {
        AbilityType = abilityType;
    }
}

/// <summary>
/// Thrown when a Task fails to execute.
/// </summary>
public class TaskExecutionException : ScreenplayException
{
    /// <summary>
    /// The name of the task that failed.
    /// </summary>
    public string TaskName { get; }
    
    /// <summary>
    /// Creates a new TaskExecutionException.
    /// </summary>
    public TaskExecutionException(string actorName, string taskName, Exception innerException)
        : base(actorName, $"failed to perform task '{taskName}'", innerException)
    {
        TaskName = taskName;
    }
}

/// <summary>
/// Thrown when a Question fails to be answered.
/// </summary>
public class QuestionException : ScreenplayException
{
    /// <summary>
    /// The name of the question that failed.
    /// </summary>
    public string QuestionName { get; }
    
    /// <summary>
    /// Creates a new QuestionException.
    /// </summary>
    public QuestionException(string actorName, string questionName, Exception innerException)
        : base(actorName, $"failed to answer question '{questionName}'", innerException)
    {
        QuestionName = questionName;
    }
}
