using ImageMagick;

namespace LibroManager.Tests.E2E.Helpers
{
    public static class VisualRegressionHelper
    {
        /// <summary>
        /// Compara el screenshot con la golden usando nombre de página y nombre base para los paths.
        /// </summary>
        /// <param name="actualScreenshotPath">Ruta del screenshot capturado</param>
        /// <param name="pageName">Nombre de la página/módulo (ej: "LoginPage")</param>
        /// <param name="imageBaseName">Nombre base de la imagen (ej: "loginbox")</param>
        /// <param name="output">Output para logs (opcional)</param>
        /// <param name="threshold">Umbral de diferencia para fallar el test</param>
        public static void AssertScreenshotMatchesGolden(
            string actualScreenshotPath,
            string pageName,
            string imageBaseName,
            Xunit.Abstractions.ITestOutputHelper? output = null,
            double threshold = 0.01)
        {
            var dirInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 3 && dirInfo != null; i++)
                dirInfo = dirInfo.Parent;
            if (dirInfo == null)
                throw new DirectoryNotFoundException("No se pudo determinar la raíz del proyecto de tests.");
            var projectDir = dirInfo.FullName;

            var actualDir = Path.Combine(projectDir, pageName, "Actual");
            var goldenDir = Path.Combine(projectDir, pageName, "Golden");
            var diffDir = Path.Combine(projectDir, pageName, "Diff");
            Directory.CreateDirectory(actualDir);
            Directory.CreateDirectory(goldenDir);
            Directory.CreateDirectory(diffDir);

            var imageName = imageBaseName + ".png";
            var actualPath = Path.Combine(actualDir, imageName + "-actual.png");
            var goldenPath = Path.Combine(goldenDir, imageName);
            var diffPath = Path.Combine(diffDir, imageBaseName + "-diff.png");

            // Si el screenshot no está en la ruta esperada, lo movemos
            if (!string.Equals(actualScreenshotPath, actualPath, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(actualScreenshotPath, actualPath, true);
            }

            if (!File.Exists(goldenPath))
            {
                File.Copy(actualPath, goldenPath);
                output?.WriteLine($"[INFO] Imagen golden creada automáticamente: {goldenPath}");
                return;
            }

            using var imgGolden = new MagickImage(goldenPath);
            using var imgActualOriginal = new MagickImage(actualPath);

            MagickImage imgActual;
            if (imgActualOriginal.Width != imgGolden.Width || imgActualOriginal.Height != imgGolden.Height)
            {
                imgActual = new MagickImage(imgActualOriginal);
                imgActual.Resize(imgGolden.Width, imgGolden.Height);
            }
            else
            {
                imgActual = imgActualOriginal;
            }

            double difference = imgGolden.Compare(imgActual, ErrorMetric.RootMeanSquared);

            var diffImage = new MagickImage(MagickColors.White, imgGolden.Width, imgGolden.Height);
            int minWidth = Math.Min((int)imgGolden.Width, (int)imgActual.Width);
            int minHeight = Math.Min((int)imgGolden.Height, (int)imgActual.Height);
            for (int y = 0; y < minHeight; y++)
            {
                for (int x = 0; x < minWidth; x++)
                {
                    var pixelActual = imgActual.GetPixels().GetPixel(x, y);
                    var pixelGolden = imgGolden.GetPixels().GetPixel(x, y);
                    if (pixelActual == null || pixelGolden == null)
                        continue;
                    var colorActual = pixelActual?.ToColor() ?? new MagickColor(255, 255, 255);
                    var colorGolden = pixelGolden?.ToColor() ?? new MagickColor(255, 255, 255);
                    double pixelThreshold = 0.05;
                    var diff = Math.Abs(colorActual.R - colorGolden.R) / 255.0
                            + Math.Abs(colorActual.G - colorGolden.G) / 255.0
                            + Math.Abs(colorActual.B - colorGolden.B) / 255.0;
                    bool isActualNotWhite = colorActual.R < 250 || colorActual.G < 250 || colorActual.B < 250;
                    if (diff > pixelThreshold && isActualNotWhite)
                    {
                        diffImage.GetPixels().SetPixel(x, y, [255, 0, 0]);
                    }
                }
            }

            if (difference > threshold)
            {
                diffImage.Write(diffPath);
                throw new Xunit.Sdk.XunitException($"La imagen no coincide con la referencia. Diferencia: {difference}. Ver diff en {diffPath}");
            }
        }
    }
}
