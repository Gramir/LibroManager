using Microsoft.Playwright;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.Sockets;

namespace LibroManager.Tests.Playwright
{


    public class PlaywrightServerFixture : IAsyncDisposable, Xunit.IAsyncLifetime
    {
        private Process? _serverProcess;
        public IPlaywright? PlaywrightInstance { get; private set; }
        public IBrowser? Browser { get; private set; }
        public string BaseUrl { get; private set; }
        private int _port;

        public PlaywrightServerFixture()
        {
            _port = GetRandomUnusedPort();
            BaseUrl = $"http://localhost:{_port}";
        }

        // Este método lo llama xUnit automáticamente antes de los tests
        public async Task InitializeAsync()
        {
            // Inicia el servidor como proceso externo
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "run",
                WorkingDirectory = GetAppWorkingDirectory(),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            startInfo.EnvironmentVariables["ASPNETCORE_URLS"] = BaseUrl;
            _serverProcess = Process.Start(startInfo);

            // Si quieres logs del servidor, puedes habilitar las siguientes líneas:
            // _serverProcess.OutputDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine($"[SERVER STDOUT] {e.Data}"); };
            // _serverProcess.ErrorDataReceived += (s, e) => { if (e.Data != null) Console.WriteLine($"[SERVER STDERR] {e.Data}"); };
            // _serverProcess.BeginOutputReadLine();
            // _serverProcess.BeginErrorReadLine();

            // Espera a que el servidor esté listo
            var isReady = await WaitForServerReady(BaseUrl, timeoutSeconds: 30);
            if (!isReady)
            {
                Console.WriteLine("[ERROR] El servidor no inició correctamente. Revisa los logs anteriores.");
                throw new Exception($"El servidor no inició correctamente en {BaseUrl}");
            }

            PlaywrightInstance = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        }

        // Este método lo llama xUnit automáticamente después de los tests
        async Task Xunit.IAsyncLifetime.DisposeAsync()
        {
            await DisposeAsync();
        }

        // Crea un nuevo contexto y página para cada test
        public async Task<(IBrowserContext, IPage)> CreateTestContextAndPageAsync()
        {
            if (Browser == null)
                throw new InvalidOperationException("El navegador no está inicializado. Llama a InitializeAsync primero.");
            var context = await Browser.NewContextAsync();
            var page = await context.NewPageAsync();
            return (context, page);
        }

        // Método para limpiar la base de datos antes de cada test
        public void ResetDatabase()
        {
            // Aquí deberías implementar la lógica para restaurar el estado inicial de la base de datos
            // Ejemplo: ejecutar un script SQL, truncar tablas, restaurar snapshot, etc.
            // Este método se debe llamar en el setup de cada test
        }

        public async ValueTask DisposeAsync()
        {
            if (Browser != null)
                await Browser.CloseAsync();
            if (PlaywrightInstance != null)
                PlaywrightInstance.Dispose();
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                _serverProcess.Kill(true);
                _serverProcess.Dispose();
            }
        }

        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Any, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static string GetAppWorkingDirectory()
        {
            // Devuelve el directorio donde está el proyecto principal
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "LibroManager"));
        }

        private static async Task<bool> WaitForServerReady(string url, int timeoutSeconds = 30)
        {
            var client = new HttpClient();
            var timeout = TimeSpan.FromSeconds(timeoutSeconds);
            var start = DateTime.UtcNow;
            while (DateTime.UtcNow - start < timeout)
            {
                try
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                        return true;
                }
                catch
                {
                    // Ignorar errores mientras el servidor arranca
                }
                await Task.Delay(1000);
            }
            return false;
        }
    }
}
