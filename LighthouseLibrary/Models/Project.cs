using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.Serialization;

namespace LighthouseLibrary.Models
{
    [Serializable]
    public class Project : ISerializable
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

        public Project(SerializationInfo info, StreamingContext _)
        {
            IsNewlyCreatedProject = GetValue<bool>("IsNewlyCreatedProject");
            ProjectName = GetValue<string>("ProjectName");
            Author = GetValue<string>("Author");

            Width = GetValue<int>("Width");
            Height = GetValue<int>("Height");
            Layers = GetValue<ObservableCollection<Layer>>("Layers");

            T GetValue<T>(string name)
            {
                return (T)info.GetValue(name, typeof(T));
            }
        }

        private Project(string projectName, string author, ObservableCollection<Layer> layers, int width, int height, bool isNewlyCreatedProject)
        {
            IsNewlyCreatedProject = isNewlyCreatedProject;
            ProjectName = projectName;
            Author = author;

            Width = width;
            Height = height;

            Layers = layers;
        }

        public Bitmap RenderProject()
        {
            switch (Layers.Count)
            {
                case 0:
                    return new Bitmap(1, 1);
                case 1:
                    return Layers[0].RenderLayer();
            }

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

        public void GetObjectData(SerializationInfo info, StreamingContext _)
        {
            info.AddValue("IsNewlyCreatedProject", IsNewlyCreatedProject);
            info.AddValue("ProjectName", ProjectName);
            info.AddValue("Author", Author);
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
            info.AddValue("Layers", Layers);
        }
    }
}