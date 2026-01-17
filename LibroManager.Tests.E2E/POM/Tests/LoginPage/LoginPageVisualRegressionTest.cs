using LibroManager.Tests.E2E.Helpers;
using LoginPagePO = LibroManager.Tests.E2E.POM.Pages.LoginPage;
using Xunit.Abstractions;

namespace LibroManager.Tests.E2E.POM.Tests.LoginPage
{
    [Collection("PlaywrightServer")]
    public class LoginPageVisualRegressionTest(PlaywrightServerFixture fixture, ITestOutputHelper output) : E2ETestBase
    {
        private readonly PlaywrightServerFixture _fixture = fixture;
        private readonly ITestOutputHelper _output = output;

        private async Task<(Microsoft.Playwright.IBrowserContext context, LoginPagePO loginPage, Microsoft.Playwright.IPage page)> CreateLoginPageAsync()
        {
            var (context, page) = await _fixture.CreateTestContextAndPageAsync();
            var loginPage = new LoginPagePO(page, _fixture.BaseUrl);
            await loginPage.GotoAsync();
            return (context, loginPage, page);
        }

        [Fact(DisplayName = "Visual regression: cuadro de login debe coincidir con la referencia")]
        [UseSnapshot("db1.db")]
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
