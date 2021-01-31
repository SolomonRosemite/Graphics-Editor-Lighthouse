using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using LighthouseLibrary.Services;

namespace LighthouseLibrary.Models
{
    public class EditorState
    {
        public List<Project> States { get; }

        private int StepsBackwards { get; set; }

        public EditorState(Project project)
        {
            States = new List<Project>();
            UpdateState(project);
        }

        public void UpdateState(Project project) => States.Add(project);

        // count = 4
        // StepsBackwards = -2
        // 5, 6 -> 5, 6, 7 -> 5, 6, 7, 8 thus StepsBackwards = 0
        public ActionResponse Redo()
        {
            if (States.Count == 1)
                return new ActionResponse(false, null);

            StepsBackwards++;
            int value = StepsBackwards - 1;

            return new ActionResponse(true, States[^value]);
        }

        // count = 4
        // StepsBackwards = 0
        // 5, 6, 7, 8 -> 5, 6, 7 -> 5, 6 thus StepsBackwards = -2
        public ActionResponse Undo()
        {
            if (States.Count == 1)
                return new ActionResponse(false, null);

            StepsBackwards--;
            int value = StepsBackwards - 1;

            return new ActionResponse(true, States[^value]);
        }
    }
}