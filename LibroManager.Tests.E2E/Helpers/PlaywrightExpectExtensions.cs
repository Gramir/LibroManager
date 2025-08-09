using Microsoft.Playwright;


namespace LibroManager.Tests.E2E.Helpers
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
