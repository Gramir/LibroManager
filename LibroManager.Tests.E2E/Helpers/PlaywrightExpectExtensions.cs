using Microsoft.Playwright;


namespace LibroManager.Tests.Playwright.Helpers
{
    public static class PlaywrightExpectExtensions
    {
        public static async Task ToBeVisibleAsync(this ILocator locator)
        {
            await Assertions.Expect(locator).ToBeVisibleAsync();
        }

        public static async Task ToBeHiddenAsync(this ILocator locator)
        {
            await Assertions.Expect(locator).ToBeHiddenAsync();
        }
    }
}
