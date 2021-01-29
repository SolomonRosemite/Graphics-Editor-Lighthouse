using System.Collections.ObjectModel;
using System.Drawing;

namespace LighthouseLibrary.Models
{
    public class Project
    {
        public string ProjectName { get; private set; }
        public string Author { get; set; }
        public ObservableCollection<Layer> Layers { get; private set; }
        public bool IsNewlyCreatedProject { get; }

        public Project(string projectName, string author, ObservableCollection<Layer> layers, bool isNewlyCreatedProject)
        {
            IsNewlyCreatedProject = isNewlyCreatedProject;
            ProjectName = projectName;
            Author = author;
            Layers = layers;
        }

        public Bitmap RenderProject()
        {
            // small note...
            // Instead of Rerendering each Layer, only rerender the layers that have been changed.
            // We can to this by checking the LayerState prop
            return Layers[0].RenderLayer();

            // Todo: Render Each Layer here.
            // Once all have been rendered try to merge them or somethink... idk
            // This might be helpful: https://stackoverflow.com/a/22182016/13024474
            return null;
        }
    }
}