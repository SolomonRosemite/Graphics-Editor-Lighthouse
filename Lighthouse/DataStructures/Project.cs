using System.Collections.Generic;

namespace Lighthouse.DataStructures
{
    public class Project
    {
        public string ProjectName { get; private set; }
        public string Author { get; private set; }
        public List<Layer> Layers { get; private set; }

        public Project(string projectName, string author, List<Layer> layers)
        {
            ProjectName = projectName;
            Author = author;
            Layers = layers;
        }
    }
}