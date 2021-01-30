using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using LighthouseLibrary.Models;

namespace LighthouseLibrary.Services
{
    public static class ImportService
    {
        public static Project LoadImportedImage(string filePath)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            Layer layer = new Layer(image, UtilService.GenerateNewId(), "Layer1");

            return new Project("unnamed", null, layer, image.Width, image.Height, true);
        }

        public static Layer LoadImportedImageToLayer(string filePath, string layername)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            Layer layer = new Layer(image, UtilService.GenerateNewId(), layername);

            return layer;
        }

        public static Project LoadImportedProject(string filePath)
        {
            return null;
        }
    }
}