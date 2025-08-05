using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;

namespace LibroManager.Tests.Playwright
{
    public abstract class PlaywrightTestBase : IAsyncLifetime
    {
        protected IPlaywright? PlaywrightInstance { get; private set; }
        protected IBrowser? Browser { get; private set; }
        protected IBrowserContext? Context { get; private set; }
        protected IPage? Page { get; private set; }
        private System.Diagnostics.Process? _serverProcess;

        public async Task InitializeAsync()
        {
            // ==== INICIO: Código a eliminar cuando la app esté hosteada en la nube ====
            // Lanzar el servidor local para pruebas Playwright
            // Ajustar el directorio de trabajo al del .csproj principal
            var projectDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", ".."));
            var csprojPath = System.IO.Path.Combine(projectDir, "LibroManager.csproj");
            if (!System.IO.File.Exists(csprojPath))
                throw new System.Exception($"No se encontró el archivo de proyecto en {csprojPath}");

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
            // ==== FIN: Código a eliminar cuando la app esté hosteada en la nube ====

            // ==== INICIO: Código a eliminar cuando la app esté hosteada en la nube ====
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
                    var response = await httpClient.GetAsync("http://localhost:5049/");
                    if (response.IsSuccessStatusCode)
                    {
                        serverUp = true;
                        break;
                    }
                }
                catch { }
                await Task.Delay(delay);
                waited += delay;
            }
            if (!serverUp)
            {
                string stdOut = _serverProcess?.StandardOutput.ReadToEnd() ?? "";
                string stdErr = _serverProcess?.StandardError.ReadToEnd() ?? "";
                _serverProcess?.Kill();
                throw new System.Exception($"No se pudo iniciar el servidor para los tests Playwright.\nSTDOUT:\n{stdOut}\nSTDERR:\n{stdErr}");
            }
            // ==== FIN: Código a eliminar cuando la app esté hosteada en la nube ====

            PlaywrightInstance = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            Context = await Browser.NewContextAsync();
            Page = await Context.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            if (Context != null)
                await Context.CloseAsync();
            if (Browser != null)
                await Browser.CloseAsync();
            if (PlaywrightInstance != null)
                PlaywrightInstance.Dispose();
            // ==== INICIO: Código a eliminar cuando la app esté hosteada en la nube ====
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                _serverProcess.Kill();
                _serverProcess.Dispose();
            }
            // ==== FIN: Código a eliminar cuando la app esté hosteada en la nube ====
        }
    }
}
