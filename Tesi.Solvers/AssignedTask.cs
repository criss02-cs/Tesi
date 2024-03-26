namespace Tesi.Solvers;

public record AssignedTask(int JobId, int TaskId, int Start, int Duration) : IComparable
{
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

public record Task(int Machine, int Duration, int TaskId)
{
    public override string ToString()
    {
        return $"Machine_{Machine}_Duration_{Duration}_TaskId_{TaskId}";
    }
}

public record Job(int JobId, List<Task> Tasks)
{
    public void AddTask(Task task) => Tasks.Add(task);
    public override string ToString()
    {
        return $"Job_{JobId}";
    }
}

public record SolverResult(Dictionary<int, List<AssignedTask>> AssignedTasks, long ElapsedMilliseconds, string Status);