using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace LibroManager.Tests.Playwright
{
    public abstract class PlaywrightTestBase
    {
        protected IPlaywright PlaywrightInstance { get; }
        protected IBrowser Browser { get; }
        protected IBrowserContext Context { get; }
        protected IPage Page { get; }

        protected PlaywrightTestBase(PlaywrightServerFixture fixture)
        {
            PlaywrightInstance = fixture.PlaywrightInstance;
            Browser = fixture.Browser;
            Context = fixture.Context;
            Page = fixture.Page;
        }
    }
}
