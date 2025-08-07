using Microsoft.Playwright;
using System.Diagnostics;


namespace LibroManager.Tests.Playwright
{
    public class PlaywrightServerFixture : IAsyncDisposable
    {
        public IPlaywright PlaywrightInstance { get; private set; }
        public IBrowser Browser { get; private set; }
        public IBrowserContext Context { get; private set; }
        public IPage Page { get; private set; }

        public PlaywrightServerFixture()
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
