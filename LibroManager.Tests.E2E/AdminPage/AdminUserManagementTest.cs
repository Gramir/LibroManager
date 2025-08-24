using LibroManager.Tests.E2E.Helpers;
using LibroManager.Tests.E2E.Pages;
using Microsoft.Playwright;
using Xunit;
using System.Threading.Tasks;
using System.IO;

namespace LibroManager.Tests.E2E.AdminPage
{
    [Collection("PlaywrightServer")]
    public class AdminUserManagementTest(PlaywrightServerFixture fixture) : E2ETestBase
    {
        private readonly PlaywrightServerFixture _fixture = fixture;
        private const string StorageStatePath = "playwright/.auth/admin-state.json";

        private async Task<(IBrowserContext context, IPage page)> CreateAdminContextAsync(bool useStorageState = false)
        {
            if (_fixture.Browser == null)
                throw new InvalidOperationException("El navegador no está inicializado. Llama a InitializeAsync primero.");

            if (useStorageState && File.Exists(StorageStatePath))
            {
                var context = await _fixture.Browser.NewContextAsync(new BrowserNewContextOptions
                {
                    StorageStatePath = StorageStatePath
                });
                var page = await context.NewPageAsync();
                return (context, page);
            }
            else
            {
                var (context, page) = await _fixture.CreateTestContextAndPageAsync();
                var loginPage = new LibroManager.Tests.E2E.Pages.LoginPage(page, _fixture.BaseUrl);
                await loginPage.GotoAsync();
                await loginPage.LoginAsync("admin@libromanager.com", "Admin123!");
                await page.WaitForURLAsync(_fixture.BaseUrl + "/");
                // Guardar el estado de la sesión
                Directory.CreateDirectory(Path.GetDirectoryName(StorageStatePath)!);
                await context.StorageStateAsync(new BrowserContextStorageStateOptions { Path = StorageStatePath });
                return (context, page);
            }
        }

        [Fact(DisplayName = "Admin puede crear usuario bibliotecario y validar login")]
        public async Task Admin_Can_Create_Bibliotecario_And_Login()
        {
            var (adminContext, adminPage) = await CreateAdminContextAsync();
            var userManagementPage = new UserManagementPage(adminPage, _fixture.BaseUrl);
            string nombre = "Biblio Test";
            string email = "biblio.test@libromanager.com";
            string password = "Biblio123!";
            string rol = "Bibliotecario";

            await RunWithReportAsync(
                adminContext,
                adminPage,
                nameof(Admin_Can_Create_Bibliotecario_And_Login) + "_CreateUser",
                async () =>
                {
                    await userManagementPage.GotoAsync();
                    await userManagementPage.CreateUserAsync(nombre, email, password, rol);
                    bool creado = false;
                    for (int i = 0; i < 5; i++)
                    {
                        await Task.Delay(500);
                        creado = await userManagementPage.IsUserCreatedAsync(email);
                        if (creado) break;
                    }
                    Assert.True(creado, "El usuario bibliotecario no aparece en la tabla después de crearlo");
                });
            await adminContext.CloseAsync();

            // Iniciar sesión como bibliotecario
            var (context, page) = await _fixture.CreateTestContextAndPageAsync();
            await RunWithReportAsync(
                context,
                page,
                nameof(Admin_Can_Create_Bibliotecario_And_Login) + "_LoginAsBiblio",
                async () =>
                {
                    var loginPage = new LibroManager.Tests.E2E.Pages.LoginPage(page, _fixture.BaseUrl);
                    await loginPage.GotoAsync();
                    await loginPage.LoginAsync(email, password);
                    await page.WaitForURLAsync(_fixture.BaseUrl + "/");
                    var mainPage = new LibroManager.Tests.E2E.Pages.MainPage(page, _fixture.BaseUrl);
                    await mainPage.UserLogged.ToBeVisibleAsync();
                    string usuarioLogueado = await mainPage.UserLogged.InnerTextAsync();
                    Assert.Contains(nombre, usuarioLogueado, StringComparison.OrdinalIgnoreCase);
                });
            await context.CloseAsync();
        }
    }
}
