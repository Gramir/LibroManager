using LibroManager.Tests.E2E.Helpers;

namespace LibroManager.Tests.E2E.LoginPage
{
    [Collection("PlaywrightServer")]
    public class LoginPageTest(PlaywrightServerFixture fixture)
    {
        private readonly PlaywrightServerFixture _fixture = fixture;

        [Fact(DisplayName = "El admin puede iniciar sesión correctamente")]
        public async Task Admin_Should_Login_Successfully()
        {
            var (context, page) = await _fixture.CreateTestContextAndPageAsync();
            var loginPage = new Pages.LoginPage(page, _fixture.BaseUrl);
            await loginPage.GotoAsync();

            // Credenciales del admin según Program.cs
            var adminEmail = "admin@libromanager.com";
            var adminPassword = "Admin123!";

            await loginPage.LoginAsync(adminEmail, adminPassword);

            // Espera a que la navegación ocurra (redirección tras login)
            await page.WaitForURLAsync(_fixture.BaseUrl + "/");

            // Verifica que el usuario está logueado
            var mainPage = new Pages.MainPage(page, _fixture.BaseUrl);
            await mainPage.NavbarTitle.ToBeVisibleAsync();
            // Verifica que no aparece el botón de login
            await mainPage.LoginLink.ToBeHiddenAsync();
            // Verifica que el nombre del usuario (o email) aparece en la navbar
            await mainPage.UserLogged.ToBeVisibleAsync();
            // Verifica que el nombre del usuario es el correcto
            Assert.Equal(adminEmail, await mainPage.UserLogged.InnerTextAsync());

            await context.CloseAsync();
        }
    }
}
