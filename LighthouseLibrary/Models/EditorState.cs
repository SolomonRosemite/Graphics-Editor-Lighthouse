using LighthouseLibrary.Services;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LighthouseLibrary.Models
{
    public class EditorState
    {
        public List<ProjectState> States { get; }

        public EditorState() { States = new List<ProjectState>(); }

        public int UpdateState(Project project)
        {
            var p = project.DeepClone();
            var id = UtilService.GenerateNewId();

            if (States.Count == 0)
            {
                States.Add(new ProjectState(p, id, ProjectStateDifference.InitialState));
                return id;
            }

            var comparer = new ObjectsComparer.Comparer<Project>();

            // comparer.AddComparerOverride("Id", DoNotCompareValueComparer.Instance);
            comparer.Compare(project, States[^1].ReconstructProject(), out var differences);

            var res = Verify();

            switch (res)
            {
                case ProjectStateDifference.Layers:
                    States.Add(new ProjectState(p, id, ProjectStateDifference.Layers));
                    break;
                case ProjectStateDifference.InitialState:
                    throw new Exception("ProjectStateDifference should not be assigned to InitialState");
                case ProjectStateDifference.None:
                    throw new Exception();
                default:
                    throw new ArgumentOutOfRangeException(nameof(res), differences.ToString());
            }
            // Verifies if all the differences are from the same type of ProjectStateDifference
            // If so we save the State, else we throw an exception...
            ProjectStateDifference Verify()
            {
                if (!differences.Any())
                    throw new Exception("No Differences");

                ProjectStateDifference diff = ProjectStateDifference.None;

                foreach (var value in differences)
                {
                    try
                    {
                        var psd = (ProjectStateDifference) Enum.Parse(
                            typeof(ProjectStateDifference),
                            CorrectPathName(value.MemberPath),
                            true);

                        if (diff != psd && diff != ProjectStateDifference.None)
                            throw new Exception("Got Differences between different types");

                        diff = psd;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Couldn't Parse Enum", e);
                    }
                }

                return diff;
            }

            string CorrectPathName(string s)
            {
                if (s.StartsWith("Layers["))
                    return "Layers";

                return s;
            }

            return id;
        }

        public ActionResponse Redo(int stateId)
        {
            var index = States.FindIndex(s => s.Id == stateId);

            if (index == States.Count - 1)
                return new ActionResponse(false, null, int.MaxValue);

            return new ActionResponse(true, States[index + 1].ReconstructProject(), States[index + 1].Id);
        }

        public ActionResponse Undo(int stateId)
        {
            var index = States.FindIndex(s => s.Id == stateId);


            if (index == 0)
                return new ActionResponse(false, null, int.MaxValue);

            return new ActionResponse(true, States[index - 1].ReconstructProject(), States[index - 1].Id);
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