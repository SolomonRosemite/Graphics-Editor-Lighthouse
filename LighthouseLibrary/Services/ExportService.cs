using System.Drawing;
using System.IO;

namespace LighthouseLibrary.Services
{
    public static class ExportService
    {
        public static Image ExportImage(byte[] layer)
        {
            using var ms = new MemoryStream(layer);
            return Image.FromStream(ms);
        }
    }
}