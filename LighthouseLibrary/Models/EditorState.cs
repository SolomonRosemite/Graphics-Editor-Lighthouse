using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;
using LighthouseLibrary.Services;
using ObjectsComparer;

namespace LighthouseLibrary.Models
{
    public class EditorState
    {
        public List<ProjectState> States { get; }

        private int StepsBackwards { get; set; }

        public EditorState() { States = new List<ProjectState>(); }

        public void UpdateState(Project project)
        {
            var p = project.DeepClone();

            if (States.Count == 0)
            {
                States.Add(new ProjectState(p, UtilService.GenerateNewId(), ProjectStateDifference.InitialState));
                return;
            }

            var comparer = new ObjectsComparer.Comparer<Project>();

            // comparer.AddComparerOverride("Id", DoNotCompareValueComparer.Instance);
            comparer.Compare(project, States[^1].ReconstructProject(), out var differences);

            string prev = "";
            int count = 1;
            foreach (var val in differences)
            {
                var statePath = CorrectPathName(val.MemberPath);

                // There should only be one change at a time. If we got more then one we throw an Exception.
                // Unless they are the same.
                if (count++ > 1)
                {
                    if (prev == statePath)
                        return;

                    LogDiff(val);
                    throw new Exception("Unexpected amount of Differences.");
                }

                ProjectStateDifference stateDifference;
                try
                {
                    Type type = typeof(ProjectStateDifference);
                    prev = statePath;
                    stateDifference = (ProjectStateDifference) Enum.Parse(type, statePath, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                // If we have multiple differences of the same ProjectStateDifference this gets executed
                // multiple times which is very expensive...

                // Todo: Optimize at sometime <3
                switch (stateDifference)
                {
                    case ProjectStateDifference.Layers:
                        States.Add(new ProjectState(p, UtilService.GenerateNewId(), ProjectStateDifference.Layers));
                        break;
                    default:
                        LogDiff(val);
                        throw new ArgumentOutOfRangeException(nameof(val), val.ToString());
                }

                LogDiff(val);
            }

            void LogDiff(Difference val)
            {
                Console.WriteLine($"{count}: ");
                Console.WriteLine("New Val: " + val.Value1);
                Console.WriteLine("Old Val: " + val.Value2);
                Console.WriteLine("Diff: " + val.DifferenceType);
                Console.WriteLine("MemberPath: " + val.MemberPath);
                Console.WriteLine("------------------------------------------------------");
            }

            string CorrectPathName(string s)
            {
                if (s.StartsWith("Layers["))
                    return "Layers";

                return s;
            }
        }

        public ActionResponse Redo()
        {
            // Todo: Fix this later
            if (States.Count == 1)
                return new ActionResponse(false, null);

            StepsBackwards++;
            int value = StepsBackwards - 1;

            return new ActionResponse(true, States[^value].ReconstructProject());
        }

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

            return new ActionResponse(true, States[States.Count + value].ReconstructProject());
        }

        public class ProjectState
        {
            public int Id { get; }
            private Project Project { get; }

            public ProjectState(Project project, int id , ProjectStateDifference difference)
            {
                // Foreach ProjectStateDifference we should only make a copy of the that thing that has changed
                // For example: If we change the Project name there is no need to copy the Layers...

                // Todo: Implement...
                switch (difference)
                {
                    case ProjectStateDifference.Layers:
                        break;
                    case ProjectStateDifference.InitialState:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(difference), difference, null);
                }
                Project = project;
                Id = id;
            }

            public Project ReconstructProject()
            {
                return Project;
            }
        }
    }
}