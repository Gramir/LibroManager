using Xunit;


namespace LibroManager.Tests.Playwright
{
    public class MainPageTest : PlaywrightTestBase
    {
        [Fact]
        public async Task MainPage_Should_Display_Title_And_LoginLink()
        {
            var mainPage = new Pages.MainPage(Page!);
            await mainPage.GotoAsync();

            // Verifica el título de la navbar
            Assert.True(await mainPage.NavbarTitle.IsVisibleAsync());
            // Verifica el encabezado principal
            Assert.True(await mainPage.MainHeader.IsVisibleAsync());
            // Verifica el enlace de login
            Assert.True(await mainPage.LoginLink.IsVisibleAsync());
            // Verifica la URL del enlace
            var href = await mainPage.LoginLink.GetAttributeAsync("href");
            Assert.True(href == "/Account/Login" || href == "Account/Login", $"El href del botón de login es inesperado: {href}");
        }
    }
}
