using Microsoft.Playwright;
using System.Threading.Tasks;

namespace LibroManager.Tests.Playwright.Pages
{
    public class MainPage
    {
        private readonly IPage _page;

        public ILocator NavbarTitle { get; }
        public ILocator MainHeader { get; }
        public ILocator LoginLink { get; }

        public MainPage(IPage page)
        {
            _page = page;
            NavbarTitle = _page.Locator("a.navbar-brand");
            MainHeader = _page.Locator("h1");
            LoginLink = _page.Locator("a.btn.btn-primary:has-text('Iniciar Sesión')");
        }

        public async Task GotoAsync()
        {
            await _page.GotoAsync("http://localhost:5049/");
        }
    }
}
