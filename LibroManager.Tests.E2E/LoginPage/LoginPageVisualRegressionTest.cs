using LibroManager.Tests.E2E.Helpers;
using Xunit.Abstractions;

namespace LibroManager.Tests.E2E.LoginPage
{
    [Collection("PlaywrightServer")]
    public class LoginPageVisualRegressionTest : E2ETestBase
    {
        private readonly PlaywrightServerFixture _fixture;
        private readonly Xunit.Abstractions.ITestOutputHelper _output;

        public LoginPageVisualRegressionTest(PlaywrightServerFixture fixture, Xunit.Abstractions.ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        private async Task<(Microsoft.Playwright.IBrowserContext context, Pages.LoginPage loginPage, Microsoft.Playwright.IPage page)> CreateLoginPageAsync()
        {
            var (context, page) = await _fixture.CreateTestContextAndPageAsync();
            var loginPage = new Pages.LoginPage(page, _fixture.BaseUrl);
            await loginPage.GotoAsync();
            return (context, loginPage, page);
        }

        [Fact(DisplayName = "Visual regression: cuadro de login debe coincidir con la referencia")]
        [Helpers.UseSnapshot("db1.db")]
        public async Task LoginBox_Should_Match_Golden_Image()
        {
            var (context, loginPage, page) = await CreateLoginPageAsync();
            await RunWithReportAsync(
                context,
                page,
                nameof(LoginBox_Should_Match_Golden_Image),
                async () =>
                {
                    // Captura screenshot en cualquier ruta temporal
                    var tempScreenshotPath = Path.GetTempFileName() + ".png";
                    await loginPage.TakeLoginBoxScreenshotAsync(tempScreenshotPath);

                    VisualRegressionHelper.AssertScreenshotMatchesGolden(
                        tempScreenshotPath,
                        "LoginPage",      // nombre de la página
                        "loginbox",       // nombre base de la imagen
                        _output,
                        0.01 // umbral ajustable
                    );
                });
        }
    }
}
