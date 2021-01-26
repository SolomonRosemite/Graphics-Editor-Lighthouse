using System.Collections.Generic;
using System.Drawing;

namespace LighthouseLibrary.Models
{
    public class Project
    {
        public string ProjectName { get; private set; }
        public string Author { get; private set; }
        public List<Layer> Layers { get; private set; }
        public bool IsNewlyCreatedProject { get; }

        public Project(string projectName, string author, List<Layer> layers, bool isNewlyCreatedProject)
        {
            IsNewlyCreatedProject = isNewlyCreatedProject;
            ProjectName = projectName;
            Author = author;
            Layers = layers;
        }

        public Bitmap RenderProject()
        {
            return Layers[0].RenderLayer();

            // Todo...
            return null;
        }
    }
}