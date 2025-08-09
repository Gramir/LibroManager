using Microsoft.Playwright;

namespace LibroManager.Tests.Playwright.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;
        private readonly string _baseUrl;

        public ILocator EmailInput { get; }
        public ILocator PasswordInput { get; }
        public ILocator LoginButton { get; }
        public ILocator ErrorMessage { get; }

        public LoginPage(IPage page, string baseUrl)
        {
            _page = page;
            _baseUrl = baseUrl.TrimEnd('/');
            EmailInput = _page.Locator("input[autocomplete='username']");
            PasswordInput = _page.Locator("input[autocomplete='current-password']");
            LoginButton = _page.Locator("button[type='submit']");
            ErrorMessage = _page.Locator(".alert-danger");
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
    }
}
