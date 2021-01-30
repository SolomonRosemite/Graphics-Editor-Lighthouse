using System;
using System.Drawing;
using LighthouseLibrary.Models;
using System.Drawing.Imaging;

namespace LighthouseLibrary.Services
{
    public static class ExportService
    {
        public static string ExportImage(Project project, ExportType type, string path)
        {
            return type switch
            {
                ExportType.Png => ExportAsPng(project, path),
                ExportType.Jpeg => ExportAsJpeg(project, path),
                ExportType.Bmp => ExportAsBitmap(project, path),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private static string ExportAsPng(Project project, string path)
        {
            Bitmap image = project.RenderProject();
            image.Save(path, ImageFormat.Png);

            return path;
        }

        private static string ExportAsJpeg(Project project, string path)
        {
            Bitmap image = project.RenderProject();
            image.Save(path, ImageFormat.Jpeg);

            return path;
        }

        private static string ExportAsBitmap(Project project, string path)
        {
            Bitmap image = project.RenderProject();
            image.Save(path, ImageFormat.Bmp);

            return path;
        }
    }
}