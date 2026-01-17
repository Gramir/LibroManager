namespace LibroManager.Tests.E2E.Screenplay.Tests;

using Xunit;
using LibroManager.Tests.E2E.Screenplay.Core.Questions;
using LibroManager.Tests.E2E.Screenplay.Tasks;
using LibroManager.Tests.E2E.Screenplay.UI;
using LibroManager.Tests.E2E.Helpers;

/// <summary>
/// Screenplay tests for the Main Page functionality.
/// These tests verify the main page displays correctly for unauthenticated users.
/// 
/// Replicates POM tests from POM/Tests/MainPage/MainPageTest.cs
/// using the Screenplay pattern for improved readability and maintainability.
/// 
/// Key Screenplay concepts demonstrated:
/// - Tasks: NavigateToPage.Home() for semantic navigation
/// - Questions: TheAttribute for querying element attributes
/// - UI Models: MainPageUI for element locators
/// </summary>
public class MainPageScreenplayTest : ScreenplayTestBase
{
    /// <summary>
    /// Creates a new MainPageScreenplayTest with the fixture.
    /// Initializes the database snapshot for consistent test data.
    /// </summary>
    /// <param name="fixture">The Playwright server fixture for browser management</param>
    public MainPageScreenplayTest(PlaywrightServerFixture fixture) : base(fixture)
    {
        // Use the same database snapshot as the POM tests for fair comparison
        PlaywrightServerFixture.NextSnapshotName = "db1.db";
    }

    /// <summary>
    /// Tests that the main page displays the title and login link correctly.
    /// 
    /// This test demonstrates using Questions to query the page state:
    /// - TheAttribute: Get attribute values for verification (href link)
    /// - Web-first assertions: ToBeVisibleAsync() waits for elements to appear
    /// 
    /// Test flow:
    /// 1. Navigate to the home page as a guest user
    /// 2. Verify the navbar title is visible
    /// 3. Verify the main header is visible
    /// 4. Verify the login link is visible
    /// 5. Verify the login link href points to the correct URL
    /// 
    /// Equivalent to POM test: MainPageTest.MainPage_Should_Display_Title_And_LoginLink
    /// </summary>
    [Fact(DisplayName = "La página principal muestra título y enlace de login (Screenplay)")]
    public async Task MainPage_Should_Display_Title_And_LoginLink()
    {
        await RunScreenplayTestWithPageAsync(
            actorName: "Guest",
            testName: nameof(MainPage_Should_Display_Title_And_LoginLink),
            screenplay: async (actor, page) =>
            {
                // GIVEN: A guest user navigates to the home page
                // The NavigateToPage task provides semantic navigation
                await actor.AttemptsToAsync(NavigateToPage.Home());

                // THEN: The navbar title should be visible
                // Web-first assertions automatically wait for the element to appear
                await MainPageUI.NavbarTitle(page).ToBeVisibleAsync();

                // AND: The main header should be visible
                await MainPageUI.MainHeader(page).ToBeVisibleAsync();

                // AND: The login link should be visible
                await MainPageUI.LoginLink(page).ToBeVisibleAsync();

                // AND: The login link should point to the login page
                // Using TheAttribute question to query the href attribute
                var href = await actor.AsksForAsync(
                    TheAttribute.Of(MainPageUI.LoginLink(page)).Named("href")
                );

                // Validate the href value (may or may not have leading slash)
                Assert.True(
                    href == "/Account/Login" || href == "Account/Login",
                    $"Expected href to be '/Account/Login' or 'Account/Login', but got: {href}"
                );
            });
    }
}
