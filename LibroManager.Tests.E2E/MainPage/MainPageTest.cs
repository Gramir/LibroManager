using LibroManager.Tests.E2E.Helpers;
using Microsoft.Playwright;

namespace LibroManager.Tests.E2E.MainPage
{
    [Collection("PlaywrightServer")]
    public class MainPageTest
    {
        private readonly PlaywrightServerFixture _fixture;

        public MainPageTest(PlaywrightServerFixture fixture)
        {
            PlaywrightServerFixture.NextSnapshotName = "db1.db";
            _fixture = fixture;
        }

        private async Task<(IBrowserContext context, Pages.MainPage mainPage)> CreateMainPageAsync()
        {
            var (context, page) = await _fixture.CreateTestContextAndPageAsync();
            var mainPage = new Pages.MainPage(page, _fixture.BaseUrl);
            await mainPage.GotoAsync();
            return (context, mainPage);
        }

        [Fact(DisplayName = "La página principal muestra título y enlace de login")]
        public async Task MainPage_Should_Display_Title_And_LoginLink()
        {
            var (context, mainPage) = await CreateMainPageAsync();

            // Web-first assertions: esperan automáticamente a que el elemento sea visible
            await mainPage.NavbarTitle.ToBeVisibleAsync();
            await mainPage.MainHeader.ToBeVisibleAsync();
            await mainPage.LoginLink.ToBeVisibleAsync();

            // Validar href del enlace de login
            var href = await mainPage.LoginLink.GetAttributeAsync("href");
            Assert.True(href == "/Account/Login" || href == "Account/Login", $"El href del botón de login es inesperado: {href}");

            await context.CloseAsync();
        }

    }
}
