using LighthouseLibrary.Models;
using System.Drawing;
using System.IO;
using System;
using LighthouseLibrary.Models.Metadata;

namespace LighthouseLibrary.Services
{
    public static class ImportService
    {
        public static Project LoadImportedImage(string filePath)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            var id = UtilService.GenerateNewId();
            var projectFolder = SetupProjectFolder();
            var target = CopyImageToProject(id, projectFolder, filePath);

            Layer layer = new Layer(image, id, "Layer1", target, new LayerMetadata(image.Width, image.Height));

            return new Project("unnamed", null, layer, image.Width, image.Height, true, projectFolder);
        }

        public static Layer LoadImportedImageToLayer(string filePath, string layerName, Project project)
        {
            Bitmap image;
            using (var bmpTemp = new Bitmap(filePath))
                image = new Bitmap(bmpTemp);

            var id = UtilService.GenerateNewId();

            var target = CopyImageToProject(id ,project.ProjectFolder, filePath);

            Layer layer = new Layer(image, UtilService.GenerateNewId(), layerName, target, new LayerMetadata(image.Width, image.Height));

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

        private static string CopyImageToProject(int id, string projectFolder, string filePath)
        {
            var target = $"{projectFolder}{id}.{filePath.Split('.')[^1]}";

            File.Copy(filePath, target);

            return target;
        }
    }
}