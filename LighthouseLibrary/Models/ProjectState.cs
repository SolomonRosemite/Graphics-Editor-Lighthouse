namespace LighthouseLibrary.Models
{
    public class ProjectState
    {
        public ProjectState(Project project, int id)
        {
            Project = project;
            Id = id;
        }

        public Project Project { get; }
        public int Id { get; }
    }
}