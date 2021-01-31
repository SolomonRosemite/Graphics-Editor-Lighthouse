using System.Diagnostics.CodeAnalysis;

namespace LighthouseLibrary.Models
{
    public class ActionResponse
    {
        public ActionResponse(bool successful, Project projectState)
        {
            Successful = successful;
            ProjectState = projectState;
        }

        public bool Successful { get; }

        [MaybeNull]
        public Project ProjectState { get; }
    }
}