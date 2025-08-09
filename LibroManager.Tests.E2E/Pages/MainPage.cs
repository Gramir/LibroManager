using Microsoft.Playwright;

namespace LibroManager.Tests.E2E.Pages
{
    public class MainPage
    {
        private readonly IPage _page;
        private readonly string _baseUrl;

        public ILocator NavbarTitle { get; }
        public ILocator MainHeader { get; }
        public ILocator LoginLink { get; }
        public ILocator UserLogged { get; }

        public MainPage(IPage page, string baseUrl)
        {
            _page = page;
            _baseUrl = baseUrl.TrimEnd('/');
            NavbarTitle = _page.Locator("a.navbar-brand");
            MainHeader = _page.Locator("h1");
            LoginLink = _page.Locator("a.btn.btn-primary:has-text('Iniciar Sesión')");
            UserLogged = _page.Locator("span.user-name");
        }

        public async Task GotoAsync()
        {
            await _page.GotoAsync(_baseUrl + "/");
        }
    }
}
