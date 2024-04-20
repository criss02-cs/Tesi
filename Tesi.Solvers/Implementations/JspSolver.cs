using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi.Solvers.Implementations;
public class JspSolver : ISolver
{
    public int Horizon { get; set; }
    public int NumMachines { get; set; }
    public int[] AllMachines { get; set; }
    public SolverResult Solve(List<Job> jobs)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        // Find the maximum machine number across all tasks in all jobs
        var maxMachine = jobs.SelectMany(job => job.Tasks).Max(task => task.Machine);
        // Initialize an array to keep track of the end times for each machine
        var machineEndTimes = new int[maxMachine + 1];

        var assignedJobs = new Dictionary<int, List<AssignedTask>>();
        // Iterate over each job
        foreach (var job in jobs)
        {
            // Initialize a variable to keep track of the end time of the previous task
            var previousTaskEndTime = 0;
            // Iterate over each task in the current job
            foreach (var task in job.Tasks)
            {
                // The start time of the current task is the maximum of the end time of the previous task
                // and the end time of the machine on which the current task is to be performed
                var startTime = Math.Max(machineEndTimes[task.Machine], previousTaskEndTime);
                // Update the end time of the machine on which the current task is to be performed
                machineEndTimes[task.Machine] = startTime + task.Duration;
                // Update the end time of the previous task
                previousTaskEndTime = startTime + task.Duration;

                if (!assignedJobs.TryGetValue(task.Machine, out var value))
                {
                    value = [];
                    assignedJobs.Add(task.Machine, value);
                }
                value.Add(new AssignedTask(jobs.IndexOf(job), task.TaskId, startTime, task.Duration));
            }
        }
        stopwatch.Stop();
        return new SolverResult(assignedJobs, stopwatch.ElapsedMilliseconds, "Feasible");
    }

}
