using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace LibroManager.Tests.Playwright
{
    public abstract class PlaywrightTestBase : IAsyncDisposable
    {
        protected IPlaywright PlaywrightInstance { get; private set; }
        protected IBrowser Browser { get; private set; }
        protected IBrowserContext Context { get; private set; }
        protected IPage Page { get; private set; }

        protected PlaywrightTestBase()
        {
            PlaywrightInstance = Microsoft.Playwright.Playwright.CreateAsync().Result;
            Browser = PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }).Result;
            Context = Browser.NewContextAsync().Result;
            Page = Context.NewPageAsync().Result;
        }

        public async ValueTask DisposeAsync()
        {
            if (Context != null)
                await Context.CloseAsync();
            if (Browser != null)
                await Browser.CloseAsync();
            if (PlaywrightInstance != null)
                PlaywrightInstance.Dispose();
        }
    }
}
