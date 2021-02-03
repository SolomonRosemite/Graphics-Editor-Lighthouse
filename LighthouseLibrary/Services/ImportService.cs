using System;
using LighthouseLibrary.Models;
using System.Drawing;
using System.IO;
using System.Net;

namespace LighthouseLibrary.Services
{
    public static class ImportService
    {
        public static Project LoadImportedImage(string filePath)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var date = DateTime.Now.ToString("g");

            int i = 1;
            var projectFolder = path + @$"\Lighthouse\Projects\Unnamed-{i}\";

            while (Directory.Exists(projectFolder))
                projectFolder = path + @$"\Lighthouse\Projects\Unnamed-{i++}\";

            var target = projectFolder + filePath.Split('\\')[^1];

            Directory.CreateDirectory(projectFolder);
            File.Copy(filePath, target, true);

            Layer layer = new Layer(image, UtilService.GenerateNewId(), "Layer1", target);

            return new Project("unnamed", null, layer, image.Width, image.Height, true, projectFolder);
        }

        public static Layer LoadImportedImageToLayer(string filePath, string layerName, Project project)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            var target = project.ProjectFolder + filePath.Split('/')[^1];
            File.Copy(filePath, target);

            Layer layer = new Layer(image, UtilService.GenerateNewId(), layerName, target);

            return layer;
        }

        public static Project LoadImportedProject(string filePath)
        {
            return null;
        }
    }
}