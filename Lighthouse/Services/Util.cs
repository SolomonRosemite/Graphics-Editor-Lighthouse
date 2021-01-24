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
        private static readonly List<int> Ids = new List<int>();
        private static readonly Random Random = new Random();

        public static int GenerateNewId()
        {
            int value = Random.Next(int.MaxValue);

            if (Ids.Contains(value))
                return GenerateNewId();

            Ids.Add(value);

            return value;
        }
    }

    public class ImportService
    {
        public static Project LoadImportedImage(string filePath)
        {
            Bitmap image = new Bitmap(filePath);
            Layer layer = new Layer(image, Util.GenerateNewId(), "Layer1");

            return new Project("unnamed", null, new List<Layer> { layer });
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