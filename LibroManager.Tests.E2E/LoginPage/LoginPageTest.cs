using LibroManager.Tests.E2E.Helpers;

namespace LibroManager.Tests.E2E.LoginPage
{
    using LibroManager.Tests.E2E.Helpers;

    [Collection("PlaywrightServer")]
    public class LoginPageTest : E2ETestBase
    {
        private readonly PlaywrightServerFixture _fixture;

        public LoginPageTest(PlaywrightServerFixture fixture)
        {
            PlaywrightServerFixture.NextSnapshotName = "db1.db";
            _fixture = fixture;
        }

        private async Task<(Microsoft.Playwright.IBrowserContext context, Pages.LoginPage loginPage, Microsoft.Playwright.IPage page)> CreateLoginPageAsync()
        {
            var (context, page) = await _fixture.CreateTestContextAndPageAsync();
            var loginPage = new Pages.LoginPage(page, _fixture.BaseUrl);
            await loginPage.GotoAsync();
            return (context, loginPage, page);
        }

        [Fact(DisplayName = "El admin puede iniciar sesión correctamente")]
        public async Task Admin_Should_Login_Successfully()
        {
            var (context, loginPage, page) = await CreateLoginPageAsync();
            var adminEmail = "admin@libromanager.com";
            var adminPassword = "Admin123!";
            await RunWithReportAsync(
                context,
                page,
                nameof(Admin_Should_Login_Successfully),
                async () =>
                {
                    await loginPage.LoginAsync(adminEmail, adminPassword);
                    await page.WaitForURLAsync(_fixture.BaseUrl + "/");
                    var mainPage = new Pages.MainPage(page, _fixture.BaseUrl);
                    await mainPage.NavbarTitle.ToBeVisibleAsync();
                    await mainPage.LoginLink.ToBeHiddenAsync();
                    await mainPage.UserLogged.ToBeVisibleAsync();
                    Assert.Equal(adminEmail, await mainPage.UserLogged.InnerTextAsync());
                });
        }
    }
}
