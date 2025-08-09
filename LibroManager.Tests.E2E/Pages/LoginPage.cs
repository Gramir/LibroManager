using Microsoft.Playwright;

namespace LibroManager.Tests.E2E.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;
        private readonly string _baseUrl;

        public ILocator EmailInput { get; }
        public ILocator PasswordInput { get; }
        public ILocator LoginButton { get; }
        public ILocator ErrorMessage { get; }
        public ILocator LoginBox { get; }

        public LoginPage(IPage page, string baseUrl)
        {
            _page = page;
            _baseUrl = baseUrl.TrimEnd('/');
            EmailInput = _page.Locator("input[autocomplete='username']");
            PasswordInput = _page.Locator("input[autocomplete='current-password']");
            LoginButton = _page.Locator("button[type='submit']");
            ErrorMessage = _page.Locator(".alert-danger");
            LoginBox = _page.GetByText("Iniciar Sesión Correo Electró");
        }

        public async Task GotoAsync()
        {
            await _page.GotoAsync(_baseUrl + "/Account/Login");
        }

        public async Task LoginAsync(string email, string password)
        {
            await EmailInput.FillAsync(email);
            await PasswordInput.FillAsync(password);
            await LoginButton.ClickAsync();
        }

        // Screenshot del cuadro de login
        public async Task<string> TakeLoginBoxScreenshotAsync(string path)
        {
            await LoginBox.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            var handle = await LoginBox.ElementHandleAsync() ?? throw new Exception("No se encontró el cuadro de login");
            await handle.ScreenshotAsync(new() { Path = path });
            return path;
        }
    }
}
