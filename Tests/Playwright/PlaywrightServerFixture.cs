using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace LibroManager.Tests.Playwright
{
    public class PlaywrightServerFixture : IAsyncDisposable
    {
        public IPlaywright PlaywrightInstance { get; private set; }
        public IBrowser Browser { get; private set; }
        public IBrowserContext Context { get; private set; }
        public IPage Page { get; private set; }
        private System.Diagnostics.Process? _serverProcess;

        public PlaywrightServerFixture()
        {
            // Lanzar el servidor local para pruebas Playwright
            var projectDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", ".."));
            var csprojPath = System.IO.Path.Combine(projectDir, "LibroManager.csproj");
            if (!System.IO.File.Exists(csprojPath))
                throw new Exception($"No se encontró el archivo de proyecto en {csprojPath}");

            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run --urls=http://localhost:5049",
                WorkingDirectory = projectDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            _serverProcess = System.Diagnostics.Process.Start(startInfo);

            // Esperar a que el servidor local esté disponible
            var httpClient = new System.Net.Http.HttpClient();
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
                _serverProcess?.Kill();
                throw new Exception($"No se pudo iniciar el servidor para los tests Playwright.\nSTDOUT:\n{stdOut}\nSTDERR:\n{stdErr}");
            }

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
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                _serverProcess.Kill();
                _serverProcess.Dispose();
            }
        }
    }
}
