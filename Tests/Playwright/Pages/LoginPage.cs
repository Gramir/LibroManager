using Microsoft.Playwright;

namespace LibroManager.Tests.Playwright.Pages
{
    public class LoginPage
    {
        private readonly IPage _page;

        public ILocator EmailInput { get; }
        public ILocator PasswordInput { get; }
        public ILocator LoginButton { get; }
        public ILocator ErrorMessage { get; }

        public LoginPage(IPage page)
        {
            _page = page;
            EmailInput = _page.Locator("input[autocomplete='username']");
            PasswordInput = _page.Locator("input[autocomplete='current-password']");
            LoginButton = _page.Locator("button[type='submit']");
            ErrorMessage = _page.Locator(".alert-danger");
        }

        public async Task GotoAsync()
        {
            await _page.GotoAsync("http://localhost:5049/Account/Login");
        }

        public async Task LoginAsync(string email, string password)
        {
            await EmailInput.FillAsync(email);
            await PasswordInput.FillAsync(password);
            await LoginButton.ClickAsync();
        }
    }
}
