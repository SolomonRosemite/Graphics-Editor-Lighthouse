using LighthouseLibrary.Services;
using System.Collections.Generic;

namespace LighthouseLibrary.Models
{
    public class EditorState
    {
        private List<ProjectSnapshot> Snapshots { get; }

        public EditorState() { Snapshots = new List<ProjectSnapshot>(); }

        public int AddNewSnapshot(Project project)
        {
            var p = UtilService.SpeedCheck(project.DeepClone);

            var id = UtilService.GenerateNewId();

            Snapshots.Add(new ProjectSnapshot(p, id));

            return id;
        }

        public ActionResponse Redo(int snapshotId)
        {
            var index = Snapshots.FindIndex(s => s.Id == snapshotId);

            if (index == Snapshots.Count - 1)
                return new ActionResponse(false, null, int.MaxValue);

            return new ActionResponse(true, Snapshots[index + 1].ReconstructProject(), Snapshots[index + 1].Id);
        }

        public ActionResponse Undo(int snapshotId)
        {
            var index = Snapshots.FindIndex(s => s.Id == snapshotId);

            if (index == 0)
                return new ActionResponse(false, null, int.MaxValue);

            return new ActionResponse(true, Snapshots[index - 1].ReconstructProject(), Snapshots[index - 1].Id);
        }
    }
}