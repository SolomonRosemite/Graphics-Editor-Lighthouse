using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace LighthouseLibrary.Models
{
    public class Project
    {
        public string ProjectName { get; private set; }
        public string Author { get; set; }
        public ObservableCollection<Layer> Layers { get; }
        public bool IsNewlyCreatedProject { get; }

        public int Width { get; }
        public int Height { get; }

        public Project(string projectName, string author, Layer layer, int width, int height, bool isNewlyCreatedProject)
        {
            IsNewlyCreatedProject = isNewlyCreatedProject;
            ProjectName = projectName;
            Author = author;

            Width = width;
            Height = height;

            Layers = new ObservableCollection<Layer> { layer };
        }

        public Bitmap RenderProject()
        {
            if (Layers.Count == 1) return Layers[0].RenderLayer();

            // Merge Layers into One Bitmap
            Bitmap res = Layers[0].RenderLayer();

            for (int i = 1; i < Layers.Count; i++)
                res = MergedBitmaps(res, Layers[i].RenderLayer());

            return res;
        }

        private Bitmap MergedBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp2 == null) return bmp1;

            Bitmap result = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(result)) {
                g.DrawImage(bmp2, Point.Empty);
                g.DrawImage(bmp1, Point.Empty);
            }
            return result;
        }
    }
}