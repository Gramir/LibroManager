using LibroManager.Tests.E2E.Helpers;
using Xunit.Abstractions;

namespace LibroManager.Tests.E2E.LoginPage
{
    [Collection("PlaywrightServer")]
    public class LoginPageVisualRegressionTest(PlaywrightServerFixture fixture, Xunit.Abstractions.ITestOutputHelper output)
    {
        private readonly PlaywrightServerFixture _fixture = fixture;
        private readonly ITestOutputHelper _output = output;

        [Fact(DisplayName = "Visual regression: cuadro de login debe coincidir con la referencia")]
        public async Task LoginBox_Should_Match_Golden_Image()
        {
            var (context, page) = await _fixture.CreateTestContextAndPageAsync();
            var loginPage = new Pages.LoginPage(page, _fixture.BaseUrl);
            await loginPage.GotoAsync();

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

            await context.CloseAsync();
        }
    }
}
