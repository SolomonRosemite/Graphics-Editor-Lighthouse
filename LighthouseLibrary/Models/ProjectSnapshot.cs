using System;
using System.Drawing;
using System.Linq;

namespace LighthouseLibrary.Models
{
    public class ProjectSnapshot
    {
        public int Id { get; }
        private Project Project { get; }

        public ProjectSnapshot(Project project, int id)
        {
            Project = project;
            Id = id;
        }

        public Project ReconstructProject()
        {
            var layers = Project.Layers.ToList();

            Project.Layers.Clear();
            foreach (var layer in layers)
                Project.Layers.Add(new Layer(new Bitmap(layer.FileName), layer.Id, layer.LayerName, layer.FileName));

            return Project;
        }
    }
}