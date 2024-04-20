using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPTANO.Modeling.Optimization;
using OPTANO.Modeling.Optimization.Enums;
using OPTANO.Modeling.Optimization.Solver;
using OPTANO.Modeling.Optimization.Solver.Z3;

namespace Tesi.Solvers.Implementations;
public class MicrosoftSolver : ISolver
{
    public int Horizon { get; set; }
    public int NumMachines { get; set; }
    public int[] AllMachines { get; set; }

    public SolverResult Solve(List<Job> jobs)
    {
        var model = new Model();
        var startTimes = new VariableCollection<Task>(
            model,
            jobs.SelectMany(x => x.Tasks),
            "startTimes",
            (t) => $"StartTime_t{t}",
            (_) => 0,
            (_) => Horizon,
            (_) => VariableType.Integer,
            null);

        // vincolo di precedenza
        foreach (var job in jobs)
        {
            for (var i = 0; i < job.Tasks.Count - 1; i++)
            {
                var current = job.Tasks[i];
                var next = job.Tasks[i + 1];
                model.AddConstraint(startTimes[next] >= startTimes[current] + current.Duration);
            }
        }

        // vincolo di non sovrapposizione
        foreach (var machine in AllMachines)
        {
            var tasksOnMachine = new List<(int i, int j)>();
            for (var i = 0; i < jobs.Count; i++)
            {
                for (var j = 0; j < jobs[i].Tasks.Count; j++)
                {
                    if (jobs[i].Tasks[j].Machine == machine)
                    {
                        tasksOnMachine.Add((i, j));
                    }
                }
            }

            for (var t1 = 0; t1 < tasksOnMachine.Count; t1++)
            {
                for (var t2 = t1 + 1; t2 < tasksOnMachine.Count; t2++)
                {
                    var (i1, j1) = tasksOnMachine[t1];
                    var (i2, j2) = tasksOnMachine[t2];
                    var task1 = jobs[i1].Tasks[j1];
                    var task2 = jobs[i2].Tasks[j2];
                    var prec = new Variable($"{i1}-{j1}_precedes_{i2}{j2}", 0, 1, VariableType.Binary);
                    model.AddConstraint(
                        startTimes[task1] + task1.Duration - Horizon * (1 - prec) <= startTimes[task2]);
                    model.AddConstraint(
                        startTimes[task2] + task2.Duration - Horizon * prec <= startTimes[task1]);
                }
            }
        }

        // funzione obiettivo
        var latestEnd = new Variable("LatestEnd", 0, Horizon, VariableType.Integer);
        foreach (var machine in AllMachines)
        {
            var taskOnMachine = jobs.SelectMany(x => x.Tasks).Where(x => x.Machine == machine).ToList();
            foreach (var task in taskOnMachine)
            {
                model.AddConstraint(latestEnd >= startTimes[task] + task.Duration, $"LatestEnd_t{task}_{machine}");
            }
        }
        model.AddObjective(new Objective(latestEnd));
        using var solver = new Z3Solver();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = solver.Solve(model);
        stopwatch.Stop();
        if (result.Status is not SolutionStatus.Optimal or SolutionStatus.Feasible)
            return new SolverResult([], stopwatch.ElapsedMilliseconds, result.Status.ToString());
        return new SolverResult(GetAssignedTask(result, jobs, startTimes), stopwatch.ElapsedMilliseconds, result.Status.ToString());
    }


    private static Dictionary<int, List<AssignedTask>> GetAssignedTask(Solution result, IEnumerable<Job> jobs,
        VariableCollection<Task> startTimes)
    {
        var assignedJobs = new Dictionary<int, List<AssignedTask>>();
        foreach (var job in jobs)
        {
            foreach (var task in job.Tasks)
            {
                var startTime = result.GetVariableValue(startTimes[task].Name) ?? 0;
                if (!assignedJobs.TryGetValue(task.Machine, out _))
                {
                    List<AssignedTask>? value = [];
                    assignedJobs.Add(task.Machine, value);
                }
                assignedJobs[task.Machine].Add(new AssignedTask(job.JobId, task.TaskId, (int)startTime, task.Duration));
                assignedJobs[task.Machine].Sort();
            }
        }

        return assignedJobs;
    }
}
