using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;
using LighthouseLibrary.Services;

namespace LighthouseLibrary.Models
{
    public class EditorState
    {
        public List<Project> States { get; }

        private int StepsBackwards { get; set; }

        public EditorState()
        {
            States = new List<Project>();
        }

        public void UpdateState(Project project)
        {
            States.Add(project.DeepClone());
        }

        // count = 4
        // StepsBackwards = -2
        // 5, 6 -> 5, 6, 7 -> 5, 6, 7, 8 thus StepsBackwards = 0
        public ActionResponse Redo()
        {
            // Todo: Fix this later
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
            // Todo: Fix this later
            if (States.Count == 1)
            // if (States.Count == 1 || StepsBackwards == 0)
                return new ActionResponse(false, null);

            StepsBackwards--;
            int value = StepsBackwards - 1;

            Console.WriteLine("value: " + value);
            Console.WriteLine("States.Count: " + States.Count);
            Console.WriteLine("StepsBackwards: " + StepsBackwards);

            return new ActionResponse(true, States[States.Count + value]);
        }
    }
}