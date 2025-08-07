using Xunit;

namespace LibroManager.Tests.Playwright
{
    [CollectionDefinition("PlaywrightServer")]
    public class PlaywrightServerCollection : ICollectionFixture<PlaywrightServerFixture>
    {
        // No se necesita implementación, solo para forzar la colección
    }
}
