using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.OrTools.Sat;

namespace Tesi.Solvers.Implementations;
public class GoogleSolver : ISolver
{
    public int Horizon { get; set; }
    public int NumMachines { get; set; }
    public int[] AllMachines { get; set; }

    public SolverResult Solve(List<Job> jobs)
    {
        var model = new CpModel();

        var allTasks = new Dictionary<Tuple<int, int>, Tuple<IntVar, IntVar, IntervalVar>>(); // (start, end, duration)
        var machineToIntervals = new Dictionary<int, List<IntervalVar>>();
        PopulateModelWithJobData(model, jobs, Horizon, allTasks, machineToIntervals);

        // vincolo di non sovrapposizione
        foreach (var machine in AllMachines)
        {
            model.AddNoOverlap(machineToIntervals[machine]);
        }

        // vincolo di precedenza
        foreach (var currentJob in jobs)
        {
            for (var taskId = 0; taskId < currentJob.Tasks.Count - 1; ++taskId)
            {
                var key1 = Tuple.Create(jobs.IndexOf(currentJob), taskId);
                var key2 = Tuple.Create(jobs.IndexOf(currentJob), taskId + 1);
                model.Add(allTasks[key2].Item1 >= allTasks[key1].Item2);
            }
        }

        // funzione obiettivo
        var objVar = model.NewIntVar(0, Horizon, "makespan");
        var ends = new List<IntVar>();
        foreach (var currentJob in jobs)
        {
            var key = Tuple.Create(jobs.IndexOf(currentJob), currentJob.Tasks.Count - 1);
            ends.Add(allTasks[key].Item2);
        }

        model.AddMaxEquality(objVar, ends);
        model.Minimize(objVar);

        var solver = new CpSolver();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var status = solver.Solve(model);
        stopwatch.Stop();
        if (status is not CpSolverStatus.Optimal or CpSolverStatus.Feasible) return new SolverResult([], stopwatch.Elapsed.TotalMilliseconds, status.ToString());
        return new SolverResult(GetAssignedTasks(solver, allTasks, jobs), stopwatch.Elapsed.TotalMilliseconds, status.ToString());
    }

    private static Dictionary<int, List<AssignedTask>> GetAssignedTasks(CpSolver solver,
        IReadOnlyDictionary<Tuple<int, int>, Tuple<IntVar, IntVar, IntervalVar>> allTasks, List<Job> jobs)
    {
        var assignedJobs = new Dictionary<int, List<AssignedTask>>();
        foreach (var job in jobs)
        {
            for (var taskId = 0; taskId < job.Tasks.Count; ++taskId)
            {
                var key = Tuple.Create(jobs.IndexOf(job), taskId);
                var task = job.Tasks[taskId];
                var start = (int)solver.Value(allTasks[key].Item1);
                if (!assignedJobs.TryGetValue(task.Machine, out var value))
                {
                    value = [];
                    assignedJobs.Add(task.Machine, value);
                }

                value.Add(new AssignedTask(jobs.IndexOf(job), taskId + 1, start, task.Duration));
                value.Sort();
            } 
        }

        return assignedJobs;
    }

    private static void PopulateModelWithJobData(CpModel model, IReadOnlyList<Job> jobs, int horizon, Dictionary<Tuple<int, int>, Tuple<IntVar, IntVar, IntervalVar>> tasksByJobAndTaskId,
        Dictionary<int, List<IntervalVar>> intervalsByMachineId)
    {
        for (var jobId = 0; jobId < jobs.Count; ++jobId)
        {
            var currentJob = jobs[jobId];
            for (var taskId = 0; taskId < currentJob.Tasks.Count; ++taskId)
            {
                var currentTask = currentJob.Tasks[taskId];
                var suffix = $"_{jobId}_{taskId}";
                var taskStart = model.NewIntVar(0, horizon, "start" + suffix);
                var taskEnd = model.NewIntVar(0, horizon, "end" + suffix);
                var taskInterval = model.NewIntervalVar(taskStart, currentTask.Duration, taskEnd, "interval" + suffix);
                var key = Tuple.Create(jobId, taskId);
                tasksByJobAndTaskId[key] = Tuple.Create(taskStart, taskEnd, taskInterval);
                if (!intervalsByMachineId.TryGetValue(currentTask.Machine, out var value))
                {
                    value = [];
                    intervalsByMachineId.Add(currentTask.Machine, value);
                }

                value.Add(taskInterval);
            }
        }
    }
}
