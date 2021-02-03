using System;
using LighthouseLibrary.Models;
using System.Drawing;
using System.IO;

namespace LighthouseLibrary.Services
{
    public static class ImportService
    {
        public static Project LoadImportedImage(string filePath)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            var projectFolder = SetupProjectFolder();

            var target = projectFolder + filePath.Split('\\')[^1];

            File.Copy(filePath, target);

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

        private static string SetupProjectFolder()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            int i = 1;
            var projectFolder = path + @$"\Lighthouse\Projects\Unnamed-{i}\";

            while (Directory.Exists(projectFolder))
                projectFolder = path + @$"\Lighthouse\Projects\Unnamed-{i++}\";

            Directory.CreateDirectory(projectFolder);

            return projectFolder;
        }
    }
}