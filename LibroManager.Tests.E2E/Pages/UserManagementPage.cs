using Microsoft.Playwright;
using System.Threading.Tasks;

namespace LibroManager.Tests.E2E.Pages
{
    public class UserManagementPage
    {
        private readonly IPage _page;
        private readonly string _baseUrl;

    public ILocator AddUserButton { get; }
    public ILocator EmailInput { get; }
    public ILocator NameInput { get; }
    public ILocator PasswordInput { get; }
    public ILocator ConfirmPasswordInput { get; }
    public ILocator RoleRadio_Bibliotecario { get; }
    public ILocator SaveButton { get; }
    public ILocator SuccessMessage { get; }
    public ILocator UserRow(string email) => _page.Locator($"tr:has(td:has-text('{email}'))");

        public UserManagementPage(IPage page, string baseUrl)
        {
            _page = page;
            _baseUrl = baseUrl.TrimEnd('/');
            AddUserButton = _page.Locator("button:has-text('Nuevo Usuario')");
            EmailInput = _page.GetByRole(AriaRole.Textbox, new() { Name = "Email*" });
            NameInput = _page.GetByRole(AriaRole.Textbox, new() { Name = "Nombre Completo*" });
            PasswordInput = _page.GetByRole(AriaRole.Textbox, new() { Name = "Contraseña*", Exact = true });
            ConfirmPasswordInput = _page.GetByRole(AriaRole.Textbox, new() { Name = "Confirmar Contraseña*" });
            RoleRadio_Bibliotecario = _page.GetByRole(AriaRole.Radio, new() { Name = "Bibliotecario" });
            SaveButton = _page.GetByRole(AriaRole.Button, new() { Name = "Guardar" });
            SuccessMessage = _page.GetByText("Usuario creado exitosamente");
        }

        public async Task GotoAsync()
        {
            await _page.GotoAsync(_baseUrl + "/Admin/Usuarios");
        }

        public async Task CreateUserAsync(string nombre, string email, string password, string rol)
        {
            await AddUserButton.ClickAsync();
            await EmailInput.FillAsync(email);
            await NameInput.FillAsync(nombre);
            await PasswordInput.FillAsync(password);
            await ConfirmPasswordInput.FillAsync(password);
            if (rol.Contains("biblio", StringComparison.CurrentCultureIgnoreCase))
                await RoleRadio_Bibliotecario.CheckAsync();
            await SaveButton.ClickAsync();
            await SuccessMessage.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        }

        public async Task<bool> IsUserCreatedAsync(string email)
        {
            return await UserRow(email).IsVisibleAsync();
        }
    }
}
