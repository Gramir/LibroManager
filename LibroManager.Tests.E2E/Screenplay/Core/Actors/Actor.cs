namespace LibroManager.Tests.E2E.Screenplay.Core.Actors;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;
using LibroManager.Tests.E2E.Screenplay.Core.Interactions;
using LibroManager.Tests.E2E.Screenplay.Core.Exceptions;
using LibroManager.Tests.E2E.Screenplay.Core.Logging;

/// <summary>
/// Represents an actor in the Screenplay pattern.
/// An Actor is the main entity that performs tasks and asks questions.
/// Actors have abilities that enable them to interact with the system under test.
/// 
/// Usage:
///   var actor = Actor.Named("Admin")
///       .WhoCan(BrowseTheWeb.Using(page, baseUrl))
///       .WithLogger(new ConsoleScreenplayLogger());
///   
///   await actor.AttemptsToAsync(Navigate.ToLoginPage());
///   var text = await actor.AsksForAsync(TheText.Of(locator));
/// </summary>
public class Actor
{
    // Dictionary to store the actor's abilities, keyed by ability type
    private readonly Dictionary<Type, IAbility> _abilities = new();
    
    // Optional logger for visibility into actor actions
    private IScreenplayLogger? _logger;
    
    /// <summary>
    /// The name of this actor (used for logging and error messages).
    /// </summary>
    public string Name { get; }
    
    // Private constructor - use Actor.Named() factory method
    private Actor(string name)
    {
        Name = name;
    }
    
    /// <summary>
    /// Creates a new Actor with the given name.
    /// This is the primary factory method for creating Actors.
    /// </summary>
    /// <param name="name">A descriptive name for the actor (e.g., "Admin", "User", "Guest")</param>
    /// <returns>A new Actor instance</returns>
    /// <example>
    /// var admin = Actor.Named("Admin");
    /// </example>
    public static Actor Named(string name) => new(name);
    
    /// <summary>
    /// Grants abilities to this actor.
    /// Abilities enable the actor to perform certain types of interactions.
    /// </summary>
    /// <param name="abilities">The abilities to grant to this actor</param>
    /// <returns>This actor (for method chaining)</returns>
    /// <example>
    /// actor.WhoCan(BrowseTheWeb.Using(page, baseUrl));
    /// </example>
    public Actor WhoCan(params IAbility[] abilities)
    {
        foreach (var ability in abilities)
        {
            _abilities[ability.GetType()] = ability;
        }
        return this;
    }
    
    /// <summary>
    /// Sets the logger for this actor.
    /// The logger will record all tasks and questions for debugging purposes.
    /// </summary>
    /// <param name="logger">The logger to use</param>
    /// <returns>This actor (for method chaining)</returns>
    public Actor WithLogger(IScreenplayLogger logger)
    {
        _logger = logger;
        return this;
    }
    
    /// <summary>
    /// Retrieves a specific ability from this actor.
    /// Throws MissingAbilityException if the actor doesn't have the requested ability.
    /// </summary>
    /// <typeparam name="T">The type of ability to retrieve</typeparam>
    /// <returns>The requested ability</returns>
    /// <exception cref="MissingAbilityException">If the actor doesn't have the ability</exception>
    public T GetAbility<T>() where T : IAbility
    {
        if (_abilities.TryGetValue(typeof(T), out var ability))
        {
            return (T)ability;
        }
        
        throw new MissingAbilityException(Name, typeof(T));
    }
    
    /// <summary>
    /// Attempts to perform one or more tasks.
    /// Tasks are executed sequentially in the order provided.
    /// </summary>
    /// <param name="tasks">The tasks to perform</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="TaskExecutionException">If any task fails</exception>
    /// <example>
    /// await actor.AttemptsToAsync(
    ///     Navigate.ToLoginPage(),
    ///     Fill.The(EmailInput).With("user@test.com"),
    ///     Click.On(LoginButton)
    /// );
    /// </example>
    public async Task AttemptsToAsync(params ITask[] tasks)
    {
        foreach (var task in tasks)
        {
            var taskName = task.GetType().Name;
            
            try
            {
                _logger?.LogTask(Name, taskName);
                await task.PerformAsAsync(this);
            }
            catch (ScreenplayException)
            {
                // Re-throw Screenplay exceptions as-is
                throw;
            }
            catch (Exception ex)
            {
                _logger?.LogError(Name, taskName, ex);
                throw new TaskExecutionException(Name, taskName, ex);
            }
        }
    }
    
    /// <summary>
    /// Asks a question and returns the answer.
    /// Questions are read-only operations that query the state of the system.
    /// </summary>
    /// <typeparam name="TAnswer">The type of the expected answer</typeparam>
    /// <param name="question">The question to ask</param>
    /// <returns>The answer to the question</returns>
    /// <exception cref="QuestionException">If the question fails to be answered</exception>
    /// <example>
    /// string userName = await actor.AsksForAsync(TheText.Of(UserNameLabel));
    /// bool isVisible = await actor.AsksForAsync(TheVisibility.Of(ErrorMessage));
    /// </example>
    public async Task<TAnswer> AsksForAsync<TAnswer>(IQuestion<TAnswer> question)
    {
        var questionName = question.GetType().Name;
        
        try
        {
            _logger?.LogQuestion(Name, questionName);
            var answer = await question.AnsweredByAsync(this);
            _logger?.LogAnswer(Name, questionName, answer);
            return answer;
        }
        catch (ScreenplayException)
        {
            // Re-throw Screenplay exceptions as-is
            throw;
        }
        catch (Exception ex)
        {
            _logger?.LogError(Name, questionName, ex);
            throw new QuestionException(Name, questionName, ex);
        }
    }
    
    /// <summary>
    /// Alias for AsksForAsync - more natural English phrasing.
    /// "The actor sees the text of the element"
    /// </summary>
    public Task<TAnswer> Sees<TAnswer>(IQuestion<TAnswer> question) 
        => AsksForAsync(question);
}
