using System.Collections.Generic;
using System.Drawing;
using LighthouseLibrary.Models;

namespace LighthouseLibrary.Services
{
    public static class ImportService
    {
        public static Project LoadImportedImage(string filePath)
        {
            Bitmap image = new Bitmap(filePath);
            Layer layer = new Layer(image, UtilService.GenerateNewId(), "Layer1");

            return new Project("unnamed", null, new List<Layer> { layer });
        }

        public static Project LoadImportedProject(string filePath)
        {
            return null;
        }
    }
}