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
        private Process? _serverProcess;

        public PlaywrightServerFixture()
        {
            // Lanzar el servidor local para pruebas Playwright
            var projectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".."));
            var csprojPath = Path.Combine(projectDir, "LibroManager.csproj");
            if (!File.Exists(csprojPath))
                throw new Exception($"No se encontró el archivo de proyecto en {csprojPath}");

            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run --urls=http://localhost:5049",
                    WorkingDirectory = projectDir,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                _serverProcess = Process.Start(startInfo);

                // Esperar a que el servidor local esté disponible
                var httpClient = new HttpClient();
                var maxWait = 20000; // 20 segundos
                var waited = 0;
                var delay = 1000;
                bool serverUp = false;
                while (waited < maxWait)
                {
                    try
                    {
                        var response = httpClient.GetAsync("http://localhost:5049/").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            serverUp = true;
                            break;
                        }
                    }
                    catch { }
                    Task.Delay(delay).Wait();
                    waited += delay;
                }
                if (!serverUp)
                {
                    string stdOut = _serverProcess?.StandardOutput.ReadToEnd() ?? "";
                    string stdErr = _serverProcess?.StandardError.ReadToEnd() ?? "";
                    try { _serverProcess?.Kill(true); } catch { }
                    _serverProcess?.Dispose();
                    throw new Exception($"No se pudo iniciar el servidor para los tests Playwright.\nSTDOUT:\n{stdOut}\nSTDERR:\n{stdErr}");
                }

                PlaywrightInstance = Microsoft.Playwright.Playwright.CreateAsync().Result;
                Browser = PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }).Result;
                Context = Browser.NewContextAsync().Result;
                Page = Context.NewPageAsync().Result;
            }
            catch
            {
                // Si ocurre cualquier excepción, asegurarse de matar el proceso del servidor
                if (_serverProcess != null && !_serverProcess.HasExited)
                {
                    try { _serverProcess.Kill(true); } catch { }
                    _serverProcess.Dispose();
                }
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (Context != null)
                await Context.CloseAsync();
            if (Browser != null)
                await Browser.CloseAsync();
            if (PlaywrightInstance != null)
                PlaywrightInstance.Dispose();
            if (_serverProcess != null)
            {
                try
                {
                    if (!_serverProcess.HasExited)
                        _serverProcess.Kill(true);
                }
                catch { }
                finally
                {
                    _serverProcess.Dispose();
                }
            }
        }
    }
}
