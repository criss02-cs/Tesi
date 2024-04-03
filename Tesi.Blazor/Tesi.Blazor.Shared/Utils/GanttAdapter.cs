using Tesi.Blazor.Shared.Models;
using Tesi.Solvers;

namespace Tesi.Blazor.Shared.Utils;

public class GanttAdapter(Dictionary<int, List<AssignedTask>> data)
{
    private readonly List<GanttData> _taskCollection = [];

    public List<GanttData> Convert()
    {
        foreach (var machine in data)
        {
            var duration = machine.Value.Sum(x => x.Duration);
            var taskData = new GanttData
            {
                Id = Guid.NewGuid(),
                Name = $"Machine {machine.Key}",
                StartDate = DateTime.Now.Date,
                Duration = $"{duration}days"
            };
            _taskCollection.Add(taskData);
            foreach (var task in machine.Value)
            {
                var t1 = new GanttData
                {
                    Id = Guid.NewGuid(),
                    Name = $"Job {task.JobId} Task {task.TaskId}",
                    StartDate = taskData.StartDate.AddDays(task.Start),
                    Duration = $"{task.Duration}days",
                    ParentId = taskData.Id,
                };
                _taskCollection.Add(t1);
            }
        }
        // ciclo per trovare i predecessori
        foreach (var task in _taskCollection)
        {
            if (task.Name is not null && task.Name.Contains("Machine")) continue;
            var jobId = task.Name?.Split(" ")[1];
            var taskId = task.Name?.Split(" ")[3];
            if (jobId == null || taskId == null) continue;
            var predecessor = GetPredecessor(int.Parse(jobId), int.Parse(taskId));
            if (predecessor != Guid.Empty)
                task.Predecessor = predecessor;
        }

        return _taskCollection;
    }
    private Guid GetPredecessor(int jobId, int taskId)
    {
        if (taskId == 1) return Guid.Empty;
        var task = data
            .SelectMany(x => x.Value)
            .FirstOrDefault(x => x.JobId == jobId && x.TaskId == taskId - 1);
        var taskData = _taskCollection
            .FirstOrDefault(x => task != null && x.Name == $"Job {jobId} Task {task.TaskId}");
        return taskData?.Id ?? Guid.Empty;
    }
}
