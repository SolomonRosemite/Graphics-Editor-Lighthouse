using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using LanguageExt;
using Lighthouse.DataStructures;

namespace Lighthouse.Services
{
    public static class Util
    {

    }

    public class ImportService
    {
        public static Project LoadImportedImage(string filePath)
        {
            Bitmap image = new Bitmap(filePath);


            Layer layer = new Layer(image, 21, "test", LayerType.Normal);

            Project project = new Project("unnamed", null, new List<Layer> { layer });
            return project;
        }

        public static Project LoadImportedProject(string filePath)
        {
            return null;
        }
    }

    public static class ExportService
    {
        public static Image ExportImage(byte[] layer)
        {
            using var ms = new MemoryStream(layer);
            return Image.FromStream(ms);
        }
    }
}