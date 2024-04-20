namespace Tesi.Solvers;

public record AssignedTask(int JobId, int TaskId, int Start, int Duration) : IComparable
{
    public string Name => $"Job_{JobId}_Task_{TaskId}"; // $"{JobId}_{TaskId}
    public int End => Start + Duration;
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is not AssignedTask otherTask) throw new ArgumentException("Object is not an AssignedTask");
        return Start != otherTask.Start
            ? Start.CompareTo(otherTask.Start)
            : Duration.CompareTo(otherTask.Duration);
    }
}

public class Task(int machine, int duration, int taskId)
{
    public string Name => $"Task {TaskId}";
    public int Machine { get; set; } = machine;
    public int Duration { get; set; } = duration;
    public int TaskId { get; set; } = taskId;
    public override string ToString()
    {
        return $"Machine_{Machine}_Duration_{Duration}_TaskId_{TaskId}";
    }
}

public record Job(int JobId, List<Task> Tasks)
{
    public string Name => $"Job {JobId}";
    public void AddTask(Task task) => Tasks.Add(task);
    public void RemoveTask(Task task) => Tasks.Remove(task);
    public void ClearTasks() => Tasks.Clear();
}

public record SolverResult(Dictionary<int, List<AssignedTask>> AssignedTasks, double ElapsedMilliseconds, string Status);