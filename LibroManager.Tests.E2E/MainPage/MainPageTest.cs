using Xunit;
using LibroManager.Tests.Playwright.Helpers;

namespace LibroManager.Tests.Playwright
{
    public class MainPageTest : PlaywrightTestBase
    {
        public MainPageTest() : base() { }

        [Fact(DisplayName = "La página principal muestra título y enlace de login")]
        public async Task MainPage_Should_Display_Title_And_LoginLink()
        {
            var mainPage = new Pages.MainPage(Page!);
            await mainPage.GotoAsync();

            // Web-first assertions: esperan automáticamente a que el elemento sea visible
            await mainPage.NavbarTitle.ToBeVisibleAsync();
            await mainPage.MainHeader.ToBeVisibleAsync();
            await mainPage.LoginLink.ToBeVisibleAsync();

            // Validar href del enlace de login
            var href = await mainPage.LoginLink.GetAttributeAsync("href");
            Assert.True(href == "/Account/Login" || href == "Account/Login", $"El href del botón de login es inesperado: {href}");
        }

        [Fact(DisplayName = "El admin puede iniciar sesión correctamente")]
        public async Task Admin_Should_Login_Successfully()
        {
            var loginPage = new Pages.LoginPage(Page!);
            await loginPage.GotoAsync();

            // Credenciales del admin según Program.cs
            var adminEmail = "admin@libromanager.com";
            var adminPassword = "Admin123!";

            await loginPage.LoginAsync(adminEmail, adminPassword);

            // Espera a que la navegación ocurra (redirección tras login)
            await Page!.WaitForURLAsync("http://localhost:5049/");

            // Verifica que el usuario está logueado
            var mainPage = new Pages.MainPage(Page!);
            await mainPage.NavbarTitle.ToBeVisibleAsync();
            // Verifica que no aparece el botón de login
            await mainPage.LoginLink.ToBeHiddenAsync();
            // Verifica que el nombre del usuario (o email) aparece en la navbar
            await mainPage.UserLogged.ToBeVisibleAsync();
            // Verifica que el nombre del usuario es el correcto
            Assert.Equal(adminEmail, await mainPage.UserLogged.InnerTextAsync());
        }
    }
}
