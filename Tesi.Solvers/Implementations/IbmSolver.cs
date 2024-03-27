
using Gurobi;
using ILOG.Concert;
using ILOG.CPLEX;
using System.Diagnostics;

namespace Tesi.Solvers.Implementations;
public class IbmSolver : ISolver
{
    public SolverResult Solve(List<Job> jobs, int horizon, int numMachines, int[] allMachines)
    {
        var cplex = new Cplex();
        var numTasks = jobs.Max(x => x.Tasks.Count);
        var (startTimes, durations, machines) = PopulateModel(cplex, jobs, numTasks, horizon);

        // vincolo di precedenza
        for (var i = 0; i < jobs.Count; i++)
        {
            for (var j = 0; j < jobs[i].Tasks.Count - 1; j++)
            {
                cplex.AddGe(startTimes[i][j + 1], cplex.Sum(startTimes[i][j], durations[i, j]));
            }
        }

        // vincolo di non sovrapposizione
        for (var m = 0; m < numMachines; m++)
        {
            var tasksOnMachine = new List<(int i, int j)>();
            for (var i = 0; i < jobs.Count; i++)
            {
                for (var j = 0; j < jobs[i].Tasks.Count; j++)
                {
                    if (machines[i, j] == m)
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
                    var prec = cplex.BoolVar($"{i1}-{j1}_precedes_{i2}{j2}");
                    var s = cplex.Sum(startTimes[i1][j1], cplex.Diff(durations[i1, j1], cplex.Prod(horizon, cplex.Diff(1, prec))));
                    cplex.AddLe(s, startTimes[i2][j2]);
                    cplex.AddLe(cplex.Sum(startTimes[i2][j2], cplex.Diff(durations[i2, j2], cplex.Prod(horizon, prec))), startTimes[i1][j1]);
                }
            }
        }

        // funzione obiettivo
        var maxCompletionTime = cplex.NumVar(0, horizon, NumVarType.Int, "maxCompletionTime");
        for (var i = 0; i < jobs.Count; i++)
        {
            for (var j = 0; j < jobs[i].Tasks.Count; j++)
            {
                // Il tempo di completamento massimo deve essere maggiore o uguale al tempo di completamento di ogni task
                cplex.AddGe(maxCompletionTime, cplex.Sum(startTimes[i][j], durations[i, j]));
            }
        }

        cplex.AddMinimize(maxCompletionTime);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        cplex.Solve();
        stopwatch.Stop();
        if (cplex.GetStatus() != Cplex.Status.Optimal || cplex.GetStatus() != Cplex.Status.Feasible)
            return new SolverResult([], stopwatch.ElapsedMilliseconds, cplex.GetStatus().ToString());
        return new SolverResult(GetAssignedTasks(cplex, jobs, numMachines, startTimes), stopwatch.ElapsedMilliseconds,
            cplex.GetStatus().ToString());
    }

    private static Dictionary<int, List<AssignedTask>> GetAssignedTasks(Cplex model, IReadOnlyList<Job> jobs, int numMachines, INumVar[][] startTimes)
    {
        var assignedJobs = new Dictionary<int, List<AssignedTask>>();
        for (var m = 0; m < numMachines; m++)
        {
            var assignedTasks = new List<AssignedTask>();
            for (var i = 0; i < jobs.Count; i++)
            {
                for (var j = 0; j < jobs[i].Tasks.Count; j++)
                {
                    if (jobs[i].Tasks[j].Machine != m) continue;
                    var startTime = model.GetValue(startTimes[i][j]);
                    assignedTasks.Add(new AssignedTask(i, j + 1, (int)startTime, jobs[i].Tasks[j].Duration));
                }
            }

            assignedTasks.Sort();
            assignedJobs.Add(m, assignedTasks);
        }

        return assignedJobs;
    }

    private (INumVar[][], int[,], int[,]) PopulateModel(Cplex model, IReadOnlyList<Job> jobs, int numTasks, int horizon)
    {
        var durations = new int[jobs.Count, numTasks];
        var machines = new int[jobs.Count, numTasks];
        var startTimes = new INumVar[jobs.Count][];
        for (var i = 0; i < jobs.Count; i++)
        {
            startTimes[i] = new INumVar[jobs[i].Tasks.Count];
            for (var j = 0; j < jobs[i].Tasks.Count; j++)
            {
                durations[i, j] = jobs[i].Tasks[j].Duration;
                machines[i, j] = jobs[i].Tasks[j].Machine;
                startTimes[i][j] = model.NumVar(0, horizon, NumVarType.Int, $"start_{i}_{j}");
            }
        }

        return (startTimes, durations, machines);
    }
}
