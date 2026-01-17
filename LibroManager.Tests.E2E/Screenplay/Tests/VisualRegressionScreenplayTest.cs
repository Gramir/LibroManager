namespace LibroManager.Tests.E2E.Screenplay.Tests;

using System.IO;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Playwright;
using LibroManager.Tests.E2E.Screenplay.Tasks;
using LibroManager.Tests.E2E.Screenplay.UI;
using LibroManager.Tests.E2E.Helpers;

/// <summary>
/// Screenplay tests for Visual Regression.
/// These tests capture screenshots and compare them against golden images
/// to detect unexpected visual changes in the UI.
/// 
/// Visual regression testing is essential for:
/// - Catching unintended UI changes during refactoring
/// - Ensuring consistent visual appearance across updates
/// - Validating CSS and layout changes
/// 
/// Replicates POM tests from:
/// - POM/Tests/LoginPage/LoginPageVisualRegressionTest.cs
/// - POM/Tests/MainPage/MainPageVisualRegressionTest.cs
/// 
/// Key Screenplay concepts demonstrated:
/// - Navigation tasks for reaching test pages
/// - UI models for targeting screenshot regions
/// - Integration with VisualRegressionHelper for image comparison
/// </summary>
public class VisualRegressionScreenplayTest : ScreenplayTestBase
{
    /// <summary>
    /// Output helper for logging test information.
    /// Used by VisualRegressionHelper to report comparison results.
    /// </summary>
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// Creates a new VisualRegressionScreenplayTest with the fixture and output helper.
    /// Initializes the database snapshot for consistent test data.
    /// </summary>
    /// <param name="fixture">The Playwright server fixture for browser management</param>
    /// <param name="output">The xUnit test output helper for logging</param>
    public VisualRegressionScreenplayTest(PlaywrightServerFixture fixture, ITestOutputHelper output)
        : base(fixture)
    {
        _output = output;
        // Use the same database snapshot as the POM tests for fair comparison
        PlaywrightServerFixture.NextSnapshotName = "db1.db";
    }

    /// <summary>
    /// Tests that the login box matches the golden image.
    /// 
    /// This test captures a screenshot of the login form area and compares
    /// it against a reference image to detect any visual changes.
    /// 
    /// Test flow:
    /// 1. Navigate to the login page
    /// 2. Wait for the login box to be visible
    /// 3. Take a screenshot of the login box element
    /// 4. Compare against the golden (reference) image
    /// 
    /// If no golden image exists, one will be created automatically.
    /// If the images differ beyond the threshold, the test fails.
    /// 
    /// Equivalent to POM test: LoginPageVisualRegressionTest.LoginBox_Should_Match_Golden_Image
    /// </summary>
    [Fact(DisplayName = "Visual regression: cuadro de login debe coincidir con la referencia (Screenplay)")]
    [UseSnapshot("db1.db")]
    public async Task LoginBox_Should_Match_Golden_Image()
    {
        await RunScreenplayTestWithPageAsync(
            actorName: "Visual Tester",
            testName: nameof(LoginBox_Should_Match_Golden_Image),
            screenplay: async (actor, page) =>
            {
                // GIVEN: Navigate to the login page
                await actor.AttemptsToAsync(NavigateToPage.Login());

                // WHEN: Wait for the login box to be visible
                var loginBox = LoginUI.LoginBox(page);
                await loginBox.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible
                });

                // AND: Take a screenshot of the login box element
                var tempScreenshotPath = Path.GetTempFileName() + ".png";

                // Get the element handle to capture just the login box
                var handle = await loginBox.ElementHandleAsync()
                    ?? throw new InvalidOperationException("Login box element not found");
                await handle.ScreenshotAsync(new ElementHandleScreenshotOptions
                {
                    Path = tempScreenshotPath
                });

                // THEN: Compare with the golden reference image
                // The helper will create the golden image if it doesn't exist
                VisualRegressionHelper.AssertScreenshotMatchesGolden(
                    tempScreenshotPath,
                    "LoginPage",      // Page name for organizing images
                    "loginbox",       // Base image name
                    _output,          // Output helper for logging
                    0.01              // Threshold for acceptable difference (1%)
                );
            });
    }

    /// <summary>
    /// Tests that the main page matches the golden image.
    /// 
    /// This test captures a full-page screenshot and compares it against
    /// a reference image to detect any visual changes.
    /// 
    /// Test flow:
    /// 1. Navigate to the main page
    /// 2. Take a full-page screenshot
    /// 3. Compare against the golden (reference) image
    /// 
    /// Full-page screenshots capture the entire page content, including
    /// any content below the fold that requires scrolling.
    /// 
    /// Equivalent to POM test: MainPageVisualRegressionTest.MainPage_Should_Match_Golden_Image
    /// </summary>
    [Fact(DisplayName = "Visual regression: la página principal debe coincidir con la referencia (Screenplay)")]
    [UseSnapshot("db1.db")]
    public async Task MainPage_Should_Match_Golden_Image()
    {
        await RunScreenplayTestWithPageAsync(
            actorName: "Visual Tester",
            testName: nameof(MainPage_Should_Match_Golden_Image),
            screenplay: async (actor, page) =>
            {
                // GIVEN: Navigate to the main page
                await actor.AttemptsToAsync(NavigateToPage.Home());

                // WHEN: Take a full page screenshot
                var tempScreenshotPath = Path.GetTempFileName() + ".png";
                await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = tempScreenshotPath,
                    FullPage = true  // Capture the entire page including below-fold content
                });

                // THEN: Compare with the golden reference image
                // The helper will create the golden image if it doesn't exist
                VisualRegressionHelper.AssertScreenshotMatchesGolden(
                    tempScreenshotPath,
                    "MainPage",       // Page name for organizing images
                    "mainpage",       // Base image name
                    _output,          // Output helper for logging
                    0.01              // Threshold for acceptable difference (1%)
                );
            });
    }
}
