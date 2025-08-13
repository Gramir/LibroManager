using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LibroManager.Tests.E2E.Helpers
{
    public static class PlaywrightReportHelper
    {
        public static async Task SaveFailureReportAsync(IBrowserContext context, IPage page, string testName, Exception ex)
        {
            Console.WriteLine($"[PlaywrightReportHelper] INICIO SaveFailureReportAsync para {testName}");
            // Usar la raíz del proyecto, no la carpeta de salida del build
            // Forzar la ruta de reportes a la carpeta del proyecto LibroManager.Tests.E2E/Reports
            // Forzar la ruta absoluta a la carpeta LibroManager.Tests.E2E/Reports en la raíz del proyecto
            string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
            string reportsRoot = Path.Combine(repoRoot, "Reports");
            Directory.CreateDirectory(reportsRoot);
            string reportDir = Path.Combine(reportsRoot, testName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            string screenshotPath = Path.Combine(reportDir, "screenshot.png");
            string errorPath = Path.Combine(reportDir, "error.txt");
            string destVideoPath = Path.Combine(reportDir, "video.webm");
            string videoLog = "";
            Directory.CreateDirectory(reportDir);
            bool screenshotOk = false;
            try
            {
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
                screenshotOk = true;
            }
            catch (Exception scrEx)
            {
                videoLog += $"Error al tomar screenshot: {scrEx.Message}\n";
            }
            try
            {
                if (page.Video != null)
                {
                    await Task.Delay(2000); // Espera más larga para asegurar que el video se guarde
                    var videoPath = await page.Video.PathAsync();
                    videoLog += $"VideoPath: {videoPath}\n";
                    if (File.Exists(videoPath))
                    {
                        try
                        {
                            File.Copy(videoPath, destVideoPath, true);
                            videoLog += "Video copiado correctamente.\n";
                        }
                        catch (Exception copyEx)
                        {
                            videoLog += $"Error al copiar el video: {copyEx.Message}\n";
                        }
                    }
                    else
                    {
                        videoLog += "El archivo de video no existe tras cerrar el contexto.\n";
                    }
                }
                else
                {
                    videoLog += "page.Video es null. No se grabó video.\n";
                }
            }
            catch (Exception vidEx)
            {
                videoLog += $"Error inesperado al manejar el video: {vidEx.Message}\n";
            }
            try
            {
                await File.WriteAllTextAsync(errorPath, $"Mensaje: {ex.Message}\nStackTrace: {ex.StackTrace}\nScreenshot: {(screenshotOk ? screenshotPath : "ERROR")}\n{videoLog}");
            }
            catch (Exception errEx)
            {
                Console.WriteLine($"[PlaywrightReportHelper] Error al escribir error.txt: {errEx.Message}");
            }
            Console.WriteLine($"[PlaywrightReportHelper] Reporte generado en: {reportDir}");
        }
    }
}
