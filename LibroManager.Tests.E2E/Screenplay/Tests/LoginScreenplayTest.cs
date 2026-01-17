namespace LibroManager.Tests.E2E.Screenplay.Tests;

using Xunit;
using LibroManager.Tests.E2E.Helpers;
using LibroManager.Tests.E2E.Screenplay.Core.Abilities;
using LibroManager.Tests.E2E.Screenplay.Core.Questions;
using LibroManager.Tests.E2E.Screenplay.Tasks;
using LibroManager.Tests.E2E.Screenplay.UI;

/// <summary>
/// Screenplay tests for the Login functionality.
/// These tests replicate the POM LoginPageTest using the Screenplay pattern.
/// 
/// The Screenplay pattern provides:
/// - Clear separation of concerns (Actors, Abilities, Tasks, Questions)
/// - Readable, intent-revealing test code
/// - Reusable components across tests
/// 
/// Compare these tests to the POM equivalents in POM/Tests/LoginPage/
/// to see the differences in approach.
/// </summary>
public class LoginScreenplayTest : ScreenplayTestBase
{
    /// <summary>
    /// Creates a new LoginScreenplayTest with the fixture.
    /// Sets the database snapshot for the test.
    /// </summary>
    public LoginScreenplayTest(PlaywrightServerFixture fixture) : base(fixture)
    {
        // Use the same database snapshot as the POM tests for fair comparison
        PlaywrightServerFixture.NextSnapshotName = "db1.db";
    }
    
    /// <summary>
    /// Tests that an admin user can successfully log in.
    /// 
    /// This test demonstrates the Screenplay pattern:
    /// - Actor: "Admin" - represents the user performing the test
    /// - Task: LoginAsUser.AsAdmin() - performs the login action
    /// - Questions: TheVisibility, TheText - verify the result
    /// 
    /// The test flow:
    /// 1. Admin actor attempts to login with admin credentials
    /// 2. Verify the navbar is visible (page loaded correctly)
    /// 3. Verify the login link is hidden (user is logged in)
    /// 4. Verify the user name display is visible
    /// 5. Verify the displayed username matches the login email
    /// </summary>
    [Fact(DisplayName = "El admin puede iniciar sesión correctamente (Screenplay)")]
    public async Task Admin_Should_Login_Successfully()
    {
        var adminEmail = "admin@libromanager.com";
        
        // Run the screenplay test
        await RunScreenplayTestWithPageAsync(
            actorName: "Admin",
            testName: nameof(Admin_Should_Login_Successfully),
            screenplay: async (actor, page) =>
            {
                // WHEN: The admin attempts to login
                // This high-level task composes multiple atomic tasks:
                // Navigate → Fill email → Fill password → Click login → Wait for URL
                await actor.AttemptsToAsync(LoginAsUser.AsAdmin());
                
                // THEN: The navbar should be visible (page loaded successfully)
                await MainPageUI.NavbarTitle(page).ToBeVisibleAsync();
                
                // AND: The login link should be hidden (user is logged in)
                await MainPageUI.LoginLink(page).ToBeHiddenAsync();
                
                // AND: The user logged indicator should be visible
                await MainPageUI.UserLogged(page).ToBeVisibleAsync();
                
                // AND: The displayed username should match the admin email
                var displayedUser = await actor.AsksForAsync(
                    TheText.Of(MainPageUI.UserLogged(page))
                );
                Assert.Equal(adminEmail, displayedUser);
            });
    }
}
