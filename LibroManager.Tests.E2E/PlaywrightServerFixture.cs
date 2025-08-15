using Microsoft.Playwright;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace LibroManager.Tests.E2E
{


    public class PlaywrightServerFixture : IAsyncDisposable, IAsyncLifetime
    {
        public static string? NextSnapshotName { get; set; } = null;
        private Process? _serverProcess;
        public IPlaywright? PlaywrightInstance { get; private set; }
        public IBrowser? Browser { get; private set; }
        public string BaseUrl { get; private set; }
        private readonly int _port;
        private static readonly List<string> _allTempDbPaths = new();
        private string _testDbPath = string.Empty;
        private string _testConnectionString = string.Empty;
        private string? _snapshotPath;

        public PlaywrightServerFixture()
        {
            NextSnapshotName = "db1.db";
            _port = GetRandomUnusedPort();
            BaseUrl = $"http://localhost:{_port}";
        }

        // Este método lo llama xUnit automáticamente antes de los tests
        public async Task InitializeAsync()
        {
            var snapshotName = NextSnapshotName;
            SetSnapshot(snapshotName);

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
            startInfo.EnvironmentVariables["ConnectionStrings__DefaultConnection"] = _testConnectionString;
            _serverProcess = Process.Start(startInfo);

            if (_serverProcess != null)
            {
                _serverProcess.OutputDataReceived += (s, e) => { };
                _serverProcess.ErrorDataReceived += (s, e) => { };
                _serverProcess.BeginOutputReadLine();
                _serverProcess.BeginErrorReadLine();
            }
            else
            {
                throw new Exception("No se pudo iniciar el proceso del servidor.");
            }

            var isReady = await WaitForServerReady(BaseUrl, timeoutSeconds: 30);
            if (!isReady)
            {
                throw new Exception($"El servidor no inició correctamente en {BaseUrl}");
            }

            PlaywrightInstance = await Microsoft.Playwright.Playwright.CreateAsync();
            Browser = await PlaywrightInstance.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true, SlowMo = 500 });
        }

        // Este método lo llama xUnit automáticamente después de los tests
        async Task IAsyncLifetime.DisposeAsync()
        {
            await DisposeAsync();
        }

        // Crea un nuevo contexto y página para cada test
        public async Task<(IBrowserContext, IPage)> CreateTestContextAndPageAsync()
        {
            if (Browser == null)
                throw new InvalidOperationException("El navegador no está inicializado. Llama a InitializeAsync primero.");
            var videoDir = Path.Combine("Reports", "Videos");
            Directory.CreateDirectory(videoDir);
            var context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                RecordVideoDir = videoDir,
                RecordVideoSize = new RecordVideoSize { Width = 1920, Height = 1080 }
            });
            var page = await context.NewPageAsync();
            return (context, page);
        }


        /// <summary>
        /// Permite seleccionar el snapshot de base de datos para el próximo test.
        /// Si es null, se usará base vacía.
        /// </summary>

        public async ValueTask DisposeAsync()
        {
            if (Browser != null)
                await Browser.CloseAsync();
            PlaywrightInstance?.Dispose();
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                _serverProcess.Kill(true);
                _serverProcess.WaitForExit(); // Esperar a que termine completamente
                _serverProcess.Dispose();
            }
            foreach (var tempPath in _allTempDbPaths)
            {
                BorrarArchivoConReintentos(tempPath);
                BorrarArchivoConReintentos(tempPath + "-wal");
                BorrarArchivoConReintentos(tempPath + "-shm");
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Permite seleccionar el snapshot de base de datos para el próximo test.
        /// Si es null, se usará base vacía.
        /// </summary>
        public void SetSnapshot(string? snapshotFileName)
        {
            var snapshotsDir = Path.Combine(GetAppWorkingDirectory(), "..", "LibroManager.Tests.E2E", "Snapshots");
            if (!Directory.Exists(snapshotsDir))
            {
                Directory.CreateDirectory(snapshotsDir);
            }
            if (!string.IsNullOrEmpty(snapshotFileName))
            {
                var fullSnapshotPath = Path.Combine(snapshotsDir, snapshotFileName);
                bool srcExists = File.Exists(fullSnapshotPath);
                long srcSize = srcExists ? new FileInfo(fullSnapshotPath).Length : -1;
                if (!srcExists)
                {
                    throw new FileNotFoundException($"Snapshot no encontrado: {fullSnapshotPath}");
                }
                _snapshotPath = fullSnapshotPath;
            }
            else
            {
                _snapshotPath = null;
            }
            var tempDbName = $"LibroManager_E2E_{Guid.NewGuid()}.db";
            _testDbPath = Path.Combine(snapshotsDir, tempDbName);
            _allTempDbPaths.Add(_testDbPath);
            if (_snapshotPath != null)
            {
                try
                {
                    File.Copy(_snapshotPath, _testDbPath, overwrite: true);
                    bool destExists = File.Exists(_testDbPath);
                    long destSize = destExists ? new FileInfo(_testDbPath).Length : -1;
                    if (!destExists)
                        throw new Exception($"El archivo destino NO existe tras copiar. Path: {_testDbPath}");
                    else if (destSize == 0)
                        throw new Exception($"El archivo destino está vacío tras copiar. Path: {_testDbPath}");
                }
                catch (Exception)
                {
                    throw;
                }
            }
            _testConnectionString = $"Data Source={_testDbPath}";
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
            return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "LibroManager"));
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

        /// <summary>
        /// Borra un archivo con reintentos por si está bloqueado temporalmente.
        /// </summary>
        private static void BorrarArchivoConReintentos(string path, int maxIntentos = 5, int msEspera = 500)
        {
            for (int intento = 0; intento < maxIntentos; intento++)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    return;
                }
                catch
                {
                    // Esperar antes de reintentar
                    Thread.Sleep(msEspera);
                }
            }
            // Si no se pudo borrar, dejarlo pasar (puedes loguear si lo deseas)
        }
    }
}
