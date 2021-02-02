using LighthouseLibrary.Models;
using System.Drawing;

namespace LighthouseLibrary.Services
{
    public static class ImportService
    {
        public static Project LoadImportedImage(string filePath)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            // Todo: Save the loaded file somewhere else in a project Directory

            string tempProjectDirectory = filePath;
            Layer layer = new Layer(image, UtilService.GenerateNewId(), "Layer1", tempProjectDirectory);

            return new Project("unnamed", null, layer, image.Width, image.Height, true);
        }

        public static Layer LoadImportedImageToLayer(string filePath, string layerName)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            // Todo: Save the loaded file somewhere else in a project Directory

            string tempProjectDirectory = filePath;
            Layer layer = new Layer(image, UtilService.GenerateNewId(), layerName, tempProjectDirectory);

            return layer;
        }

        public static Project LoadImportedProject(string filePath)
        {
            return null;
        }
    }
}