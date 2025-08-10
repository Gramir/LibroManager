using LibroManager.Tests.E2E.Helpers;
using Xunit.Abstractions;

namespace LibroManager.Tests.E2E.MainPage
{
    [Collection("PlaywrightServer")]
    public class MainPageVisualRegressionTest(PlaywrightServerFixture fixture, ITestOutputHelper output)
    {
        private readonly PlaywrightServerFixture _fixture = fixture;
        private readonly ITestOutputHelper _output = output;

        private async Task<(Microsoft.Playwright.IBrowserContext context, Pages.MainPage mainPage, Microsoft.Playwright.IPage page)> CreateMainPageAsync()
        {
            var (context, page) = await _fixture.CreateTestContextAndPageAsync();
            var mainPage = new Pages.MainPage(page, _fixture.BaseUrl);
            await mainPage.GotoAsync();
            return (context, mainPage, page);
        }

        [Fact(DisplayName = "Visual regression: la página principal debe coincidir con la referencia")]
        public async Task MainPage_Should_Match_Golden_Image()
        {
            var (context, mainPage, page) = await CreateMainPageAsync();

            // Captura screenshot de la página completa en una ruta temporal
            var tempScreenshotPath = Path.GetTempFileName() + ".png";
            await page.ScreenshotAsync(new() { Path = tempScreenshotPath, FullPage = true });

            VisualRegressionHelper.AssertScreenshotMatchesGolden(
                tempScreenshotPath,
                "MainPage",      // nombre de la página
                "mainpage",      // nombre base de la imagen
                _output,
                0.01 // umbral ajustable
            );

            await context.CloseAsync();
        }
    }
}
