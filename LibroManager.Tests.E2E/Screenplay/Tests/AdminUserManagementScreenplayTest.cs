namespace LibroManager.Tests.E2E.Screenplay.Tests;

using Xunit;
using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Core.Actors;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;
using LibroManager.Tests.E2E.Screenplay.Core.Questions;
using LibroManager.Tests.E2E.Screenplay.Tasks;
using LibroManager.Tests.E2E.Screenplay.UI;
using LibroManager.Tests.E2E.Helpers;

/// <summary>
/// Screenplay tests for Admin User Management functionality.
/// These tests verify that administrators can create and manage users.
/// 
/// Demonstrates complex Screenplay workflows involving:
/// - Multiple actors (Admin creates user, then new user logs in)
/// - High-level composed tasks (LoginAsUser, CreateUser)
/// - Verification using Questions (TheVisibility, TheText)
/// - Browser context management for session handling
/// 
/// Replicates POM tests from POM/Tests/AdminPage/AdminUserManagementTest.cs
/// using the Screenplay pattern for improved readability and maintainability.
/// </summary>
public class AdminUserManagementScreenplayTest : ScreenplayTestBase
{
    /// <summary>
    /// Creates a new AdminUserManagementScreenplayTest with the fixture.
    /// Initializes the database snapshot for consistent test data.
    /// </summary>
    /// <param name="fixture">The Playwright server fixture for browser management</param>
    public AdminUserManagementScreenplayTest(PlaywrightServerFixture fixture) : base(fixture)
    {
        // Use the same database snapshot as the POM tests for fair comparison
        PlaywrightServerFixture.NextSnapshotName = "db1.db";
    }

    /// <summary>
    /// Creates an admin actor that is already logged in.
    /// 
    /// This method:
    /// - Creates a fresh browser context
    /// - Logs in as admin using the LoginAsUser task
    /// - Returns the actor ready to perform admin actions
    /// </summary>
    /// <returns>A tuple containing the Actor, browser context, and page</returns>
    private async Task<(Actor actor, IBrowserContext context, IPage page)> CreateAdminActorAsync()
    {
        if (Fixture.Browser == null)
            throw new InvalidOperationException("Browser not initialized. Call InitializeAsync first.");

        // Create new context and perform login (consistent with POM approach)
        var (context, page) = await Fixture.CreateTestContextAndPageAsync();

        // Create the actor and perform login
        var actor = Actor.Named("Admin")
            .WhoCan(BrowseTheWeb.Using(page, Fixture.BaseUrl));
        
        await actor.AttemptsToAsync(LoginAsUser.AsAdmin());

        return (actor, context, page);
    }

    /// <summary>
    /// Tests that an admin can create a bibliotecario user and that user can login.
    /// 
    /// This is a complex end-to-end test demonstrating:
    /// 1. Admin logs in and creates a new bibliotecario user
    /// 2. Verification that the user appears in the management table
    /// 3. Admin logs out and new bibliotecario logs in
    /// 4. Verification that the bibliotecario is correctly logged in
    /// 
    /// Uses two separate browser contexts to simulate different user sessions.
    /// 
    /// Test flow:
    /// - Part 1: Admin creates the bibliotecario
    /// - Part 2: Bibliotecario logs in and verifies identity
    /// 
    /// Equivalent to POM test: AdminUserManagementTest.Admin_Can_Create_Bibliotecario_And_Login
    /// </summary>
    [Fact(DisplayName = "Admin puede crear usuario bibliotecario y validar login (Screenplay)")]
    public async Task Admin_Can_Create_Bibliotecario_And_Login()
    {
        // Test data - credentials for the new bibliotecario user
        // Using a unique email (screenplay prefix) to avoid conflicts with POM tests
        var nombre = "Biblio Test Screenplay";
        var email = "biblio.screenplay@libromanager.com";
        var password = "Biblio123!";

        // ========================================
        // PART 1: Admin creates the bibliotecario user
        // ========================================
        var (adminActor, adminContext, adminPage) = await CreateAdminActorAsync();

        await RunWithReportAsync(
            adminContext,
            adminPage,
            nameof(Admin_Can_Create_Bibliotecario_And_Login) + "_CreateUser",
            async () =>
            {
                // WHEN: Admin creates a new bibliotecario user
                // The CreateUser task handles navigation, form filling, and submission
                await adminActor.AttemptsToAsync(
                    CreateUser.WithDetails(nombre, email, password).AsBibliotecario()
                );

                // THEN: Verify the user appears in the management table
                // Poll with retries to account for async table updates
                bool userCreated = false;
                for (int i = 0; i < 5; i++)
                {
                    await Task.Delay(500);
                    userCreated = await ActorCanSeeUserInTable(adminActor, adminPage, email);
                    if (userCreated) break;
                }

                Assert.True(userCreated, "The bibliotecario user was not found in the table after creation");
            });

        // Clean up the admin context before starting the bibliotecario session
        await adminContext.CloseAsync();

        // ========================================
        // PART 2: The new bibliotecario logs in
        // ========================================
        await RunScreenplayTestWithPageAsync(
            actorName: "Bibliotecario",
            testName: nameof(Admin_Can_Create_Bibliotecario_And_Login) + "_LoginAsBiblio",
            screenplay: async (biblioActor, page) =>
            {
                // WHEN: Bibliotecario logs in with their new credentials
                await biblioActor.AttemptsToAsync(
                    LoginAsUser.WithCredentials(email, password)
                );

                // THEN: The user logged indicator should be visible
                await MainPageUI.UserLogged(page).ToBeVisibleAsync();

                // AND: The displayed name should contain the bibliotecario's name
                var displayedUser = await biblioActor.AsksForAsync(
                    TheText.Of(MainPageUI.UserLogged(page))
                );

                // Verify the displayed username matches (case-insensitive comparison)
                Assert.Contains(nombre, displayedUser, StringComparison.OrdinalIgnoreCase);
            });
    }

    /// <summary>
    /// Helper method to check if a user exists in the management table.
    /// Uses the TheVisibility question to check for the user's row.
    /// </summary>
    /// <param name="actor">The actor performing the check</param>
    /// <param name="page">The page to search on</param>
    /// <param name="email">The email of the user to find</param>
    /// <returns>True if the user row is visible, false otherwise</returns>
    private static async Task<bool> ActorCanSeeUserInTable(Actor actor, IPage page, string email)
    {
        return await actor.AsksForAsync(
            TheVisibility.Of(UserManagementUI.UserRow(page, email))
        );
    }
}
