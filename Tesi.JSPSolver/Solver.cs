namespace Tesi.JSPSolver;

/// <summary>
/// Represents a task to be performed on a machine.
/// </summary>
public class Task(int machine, int duration, int taskId)
{
    /// <summary>
    /// Gets the name of the task.
    /// </summary>
    public string Name => $"Task {TaskId}";

    /// <summary>
    /// Gets or sets the machine on which the task is to be performed.
    /// </summary>
    public int Machine { get; set; } = machine;

    /// <summary>
    /// Gets or sets the duration of the task.
    /// </summary>
    public int Duration { get; set; } = duration;

    /// <summary>
    /// Gets or sets the unique identifier of the task.
    /// </summary>
    public int TaskId { get; set; } = taskId;

    /// <summary>
    /// Gets or sets the start time of the task.
    /// </summary>
    public int StartTime { get; set; }

    /// <summary>
    /// Returns a string that represents the current task.
    /// </summary>
    public override string ToString()
    {
        return $"Machine_{Machine}_Duration_{Duration}_TaskId_{TaskId}";
    }
}

/// <summary>
/// Represents a job that consists of multiple tasks.
/// </summary>
public record Job(int JobId, List<Task> Tasks)
{
    /// <summary>
    /// Gets the name of the job.
    /// </summary>
    public string Name => $"Job {JobId}";

    /// <summary>
    /// Adds a task to the job.
    /// </summary>
    public void AddTask(Task task) => Tasks.Add(task);

    /// <summary>
    /// Removes a task from the job.
    /// </summary>
    public void RemoveTask(Task task) => Tasks.Remove(task);

    /// <summary>
    /// Clears all tasks from the job.
    /// </summary>
    public void ClearTasks() => Tasks.Clear();
}

/// <summary>
/// Represents a solver that uses the Johnson method to schedule jobs.
/// </summary>
public class Solver
{
    /// <summary>
    /// Gets or sets the list of jobs to be scheduled.
    /// </summary>
    public List<Job> Jobs { get; set; }

    /// <summary>
    /// Initializes a new instance of the Solver class.
    /// </summary>
    public Solver()
    {
        Jobs = new List<Job>();
    }

    /// <summary>
    /// Schedules the jobs using the Johnson method.
    /// </summary>
    public void JohnsonMethod()
    {
        // Find the maximum machine number across all tasks in all jobs
        var maxMachine = Jobs.SelectMany(job => job.Tasks.Select(task => task.Machine)).Max();

        // Initialize an array to keep track of the end times for each machine
        var machineEndTimes = new int[maxMachine + 1];

        // Iterate over each job
        foreach (var job in Jobs)
        {
            // Initialize a variable to keep track of the end time of the previous task
            int previousTaskEndTime = 0;

            // Iterate over each task in the current job
            foreach (var task in job.Tasks)
            {
                // The start time of the current task is the maximum of the end time of the previous task
                // and the end time of the machine on which the current task is to be performed
                task.StartTime = Math.Max(machineEndTimes[task.Machine], previousTaskEndTime);

                // Update the end time of the machine on which the current task is to be performed
                machineEndTimes[task.Machine] = task.StartTime + task.Duration;

                // Update the end time of the previous task
                previousTaskEndTime = machineEndTimes[task.Machine];
            }
        }

        // Group tasks by machine
        var tasksByMachine = Jobs.SelectMany(job => job.Tasks).GroupBy(task => task.Machine).ToList();

        // Iterate over each group of tasks
        foreach (var group in tasksByMachine)
        {
            // Print the machine number
            Console.WriteLine($"Machine: {group.Key}");

            // Iterate over each task in the current group
            foreach (var task in group)
            {
                // Find the job that contains the current task
                var jobId = Jobs.First(job => job.Tasks.Contains(task)).JobId;

                // Print the job and task IDs, and the start and end times of the task
                Console.WriteLine($"job_{jobId}_task_{task.TaskId} [{task.StartTime}, {task.StartTime + task.Duration}] \t");
            }
        }

        // Print the minimum completion time, which is the maximum end time across all machines
        Console.WriteLine($"Minimum completion time: {machineEndTimes.Max()}");
    }
}
