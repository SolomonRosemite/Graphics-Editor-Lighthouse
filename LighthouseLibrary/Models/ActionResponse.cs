using System.Diagnostics.CodeAnalysis;

namespace LighthouseLibrary.Models
{
    public class ActionResponse
    {
        public ActionResponse(bool successful, Project projectState, int stateId)
        {
            Successful = successful;
            ProjectState = projectState;
            StateId = stateId;
        }

        public int StateId { get; }
        public bool Successful { get; }

        [MaybeNull]
        public Project ProjectState { get; }
    }
}