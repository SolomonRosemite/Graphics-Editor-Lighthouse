using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
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
        public string ProjectFolder { get; set; }

        public int Width { get; }
        public int Height { get; }

        public Project(string projectName, string author, Layer layer, int width, int height, bool isNewlyCreatedProject, string projectFolder)
        {
            IsNewlyCreatedProject = isNewlyCreatedProject;
            ProjectFolder = projectFolder;
            ProjectName = projectName;
            Author = author;

            Width = width;
            Height = height;

            Layers = new ObservableCollection<Layer> { layer };
        }

        private Project(string projectName, string author, ObservableCollection<Layer> layers, int width, int height, bool isNewlyCreatedProject, string projectFolder)
        {
            IsNewlyCreatedProject = isNewlyCreatedProject;
            ProjectName = projectName;
            ProjectFolder = projectFolder;
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
                g.DrawImage(bmp2, 0, 0, bmp2.Width, bmp2.Height);
                g.DrawImage(bmp1, 0, 0, bmp1.Width, bmp1.Height);
            }

            return result;
        }

        public Project(SerializationInfo info, StreamingContext _)
        {
            IsNewlyCreatedProject = GetValue<bool>("IsNewlyCreatedProject");
            ProjectFolder = GetValue<string>("ProjectFolder");
            ProjectName = GetValue<string>("ProjectName");
            Author = GetValue<string>("Author");

            Layers = GetValue<ObservableCollection<Layer>>("Layers");
            Height = GetValue<int>("Height");
            Width = GetValue<int>("Width");

            T GetValue<T>(string name) => (T)info.GetValue(name, typeof(T));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext _)
        {
            info.AddValue("IsNewlyCreatedProject", IsNewlyCreatedProject);
            info.AddValue("ProjectFolder", ProjectFolder);
            info.AddValue("ProjectName", ProjectName);
            info.AddValue("Author", Author);
            info.AddValue("Layers", Layers);
            info.AddValue("Height", Height);
            info.AddValue("Width", Width);
        }

        public Project Clone()
        {
            return new Project(ProjectName, Author, new ObservableCollection<Layer>(Layers), Width, Height, IsNewlyCreatedProject, ProjectFolder);
        }
    }
}