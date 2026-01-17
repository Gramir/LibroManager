namespace LibroManager.Tests.E2E.Screenplay.Tests;

using Microsoft.Playwright;
using LibroManager.Tests.E2E.Helpers;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;
using LibroManager.Tests.E2E.Screenplay.Core.Logging;

/// <summary>
/// Base class for all Screenplay tests.
/// Provides common setup and helper methods for creating Actors and running tests
/// with proper reporting and cleanup.
/// 
/// This class integrates the Screenplay pattern with the existing test infrastructure:
/// - Uses PlaywrightServerFixture for browser and server management
/// - Uses E2ETestBase.RunWithReportAsync for test reporting
/// - Provides CreateActorAsync for easy Actor creation with BrowseTheWeb ability
/// 
/// Usage:
///   public class MyTest : ScreenplayTestBase
///   {
///       public MyTest(PlaywrightServerFixture fixture) : base(fixture) { }
///       
///       [Fact]
///       public async Task MyTestMethod()
///       {
///           await RunScreenplayTestAsync("ActorName", nameof(MyTestMethod), async actor =>
///           {
///               await actor.AttemptsToAsync(/* tasks */);
///               var result = await actor.AsksForAsync(/* question */);
///               Assert.Equal(expected, result);
///           });
///       }
///   }
/// </summary>
[Collection("PlaywrightServer")]
public abstract class ScreenplayTestBase : E2ETestBase
{
    /// <summary>
    /// The shared Playwright server fixture.
    /// Provides access to the browser, base URL, and test context creation.
    /// </summary>
    protected readonly PlaywrightServerFixture Fixture;
    
    /// <summary>
    /// Optional logger for Screenplay actions.
    /// Set to true to enable console logging of all Actor actions.
    /// </summary>
    protected bool EnableLogging { get; set; } = false;
    
    /// <summary>
    /// Creates a new ScreenplayTestBase with the specified fixture.
    /// </summary>
    /// <param name="fixture">The Playwright server fixture</param>
    protected ScreenplayTestBase(PlaywrightServerFixture fixture)
    {
        Fixture = fixture;
    }
    
    /// <summary>
    /// Creates a new Actor with the BrowseTheWeb ability.
    /// The Actor is configured with a fresh browser context and page.
    /// 
    /// Returns a tuple containing:
    /// - The Actor ready to perform tasks
    /// - The browser context (for cleanup)
    /// - The page (for direct access if needed)
    /// </summary>
    /// <param name="name">A descriptive name for the Actor (e.g., "Admin", "User")</param>
    /// <returns>A tuple of (Actor, IBrowserContext, IPage)</returns>
    /// <example>
    /// var (actor, context, page) = await CreateActorAsync("Admin");
    /// </example>
    protected async Task<(Actor actor, IBrowserContext context, IPage page)> CreateActorAsync(string name)
    {
        // Create a fresh browser context and page for this test
        var (context, page) = await Fixture.CreateTestContextAndPageAsync();
        
        // Create the Actor with the BrowseTheWeb ability
        var actor = Actor.Named(name)
            .WhoCan(BrowseTheWeb.Using(page, Fixture.BaseUrl));
        
        // Optionally add a logger
        if (EnableLogging)
        {
            actor.WithLogger(new ConsoleScreenplayLogger());
        }
        
        return (actor, context, page);
    }
    
    /// <summary>
    /// Runs a Screenplay test with proper setup, execution, and cleanup.
    /// This is the primary method for writing Screenplay tests.
    /// 
    /// The method:
    /// 1. Creates an Actor with the specified name
    /// 2. Executes the screenplay (test actions)
    /// 3. Handles reporting for failures
    /// 4. Cleans up the browser context
    /// </summary>
    /// <param name="actorName">The name for the Actor in this test</param>
    /// <param name="testName">The name of the test (for reporting)</param>
    /// <param name="screenplay">The async action containing the test logic</param>
    /// <example>
    /// await RunScreenplayTestAsync("Admin", nameof(Admin_Can_Login), async actor =>
    /// {
    ///     await actor.AttemptsToAsync(LoginAsUser.AsAdmin());
    ///     var isLoggedIn = await actor.AsksForAsync(TheVisibility.Of(MainPageUI.UserLogged(page)));
    ///     Assert.True(isLoggedIn);
    /// });
    /// </example>
    protected async Task RunScreenplayTestAsync(
        string actorName,
        string testName,
        Func<Actor, Task> screenplay)
    {
        // Create the Actor and get the context/page for cleanup and reporting
        var (actor, context, page) = await CreateActorAsync(actorName);
        
        // Run the test with the existing reporting infrastructure
        await RunWithReportAsync(
            context,
            page,
            testName,
            async () => await screenplay(actor)
        );
    }
    
    /// <summary>
    /// Runs a Screenplay test that needs direct access to the page.
    /// Use this variant when you need to access the IPage for additional assertions
    /// or operations not covered by the Screenplay pattern.
    /// </summary>
    /// <param name="actorName">The name for the Actor</param>
    /// <param name="testName">The name of the test</param>
    /// <param name="screenplay">The async action with Actor and IPage parameters</param>
    protected async Task RunScreenplayTestWithPageAsync(
        string actorName,
        string testName,
        Func<Actor, IPage, Task> screenplay)
    {
        var (actor, context, page) = await CreateActorAsync(actorName);
        
        await RunWithReportAsync(
            context,
            page,
            testName,
            async () => await screenplay(actor, page)
        );
    }
}
