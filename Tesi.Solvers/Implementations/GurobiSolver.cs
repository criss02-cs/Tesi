using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Gurobi;

namespace Tesi.Solvers.Implementations;
internal class GurobiSolver : ISolver
{
    public SolverResult Solve(List<Job> jobs, int horizon, int numMachines, int[] allMachines)
    {
        var env = new GRBEnv();
        var model = new GRBModel(env);
        var numTasks = jobs.Max(x => x.Tasks.Count);
        var (startTimes, durations, machines) = PopulateModel(model, jobs, numTasks, horizon);

        // vincolo di precedenza
        for (var i = 0; i < jobs.Count; i++)
        {
            for (var j = 0; j < jobs[i].Tasks.Count - 1; j++)
            {
                var leftSide = new GRBLinExpr(startTimes[i][j + 1], 1);
                var rightSide = startTimes[i][j] + durations[i, j];
                model.AddConstr(leftSide, GRB.GREATER_EQUAL, rightSide, $"precedence_{i}{j}");
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
                    var prec = model.AddVar(0, 1, 0, GRB.BINARY, $"{i1}-{j1}_precedes_{i2}{j2}");
                    var s = startTimes[i1][j1] + durations[i1, j1] - horizon * (1 - prec);
                    model.AddConstr(s, GRB.LESS_EQUAL, startTimes[i2][j2], $"non_overlap_{i1}{j1}_{i2}{j2}");
                    s = startTimes[i2][j2] + durations[i2, j2] - horizon * prec;
                    model.AddConstr(s, GRB.LESS_EQUAL, startTimes[i1][j1], $"non_overlap_{i2}{j2}_{i1}{j1}");
                }
            }
        }

        // funzione obiettivo
        var maxCompletionTime = model.AddVar(0, horizon, 0, GRB.INTEGER, "minCompletionTime");
        for (var i = 0; i < jobs.Count; i++)
        {
            for (var j = 0; j < jobs[i].Tasks.Count; j++)
            {
                var end = startTimes[i][j] + durations[i, j];
                model.AddConstr(maxCompletionTime, GRB.GREATER_EQUAL, end, $"end_{i}{j}");
            }
        }
        model.SetObjective(new GRBLinExpr(maxCompletionTime, 1), GRB.MINIMIZE);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        model.Optimize();
        stopwatch.Stop();
        if (model.Status != GRB.Status.OPTIMAL) return new SolverResult([], stopwatch.ElapsedMilliseconds, "");

        return new SolverResult(GetAssignedTasks(model, jobs, numMachines), stopwatch.ElapsedMilliseconds, nameof(GRB.Status.OPTIMAL));
    }

    private static Dictionary<int, List<AssignedTask>> GetAssignedTasks(GRBModel model, IReadOnlyList<Job> jobs, int numMachines)
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
                    var startTime = model.GetVarByName($"start_{i}_{j}").X;
                    assignedTasks.Add(new AssignedTask(i, j + 1, (int)startTime, jobs[i].Tasks[j].Duration));
                }
            }

            assignedTasks.Sort();
            assignedJobs.Add(m, assignedTasks);
        }

        return assignedJobs;
    }

    private (GRBVar[][], int[,], int[,]) PopulateModel(GRBModel model, IReadOnlyList<Job> jobs, int numTasks, int horizon)
    {
        var durations = new int[jobs.Count, numTasks];
        var machines = new int[jobs.Count, numTasks];
        var startTimes = new GRBVar[jobs.Count][];
        for (var i = 0; i < jobs.Count; i++)
        {
            startTimes[i] = new GRBVar[jobs[i].Tasks.Count];
            for (var j = 0; j < jobs[i].Tasks.Count; j++)
            {
                durations[i, j] = jobs[i].Tasks[j].Duration;
                machines[i, j] = jobs[i].Tasks[j].Machine;
                startTimes[i][j] = model.AddVar(0, horizon, 0, GRB.INTEGER, $"start_{i}_{j}");
            }
        }

        return (startTimes, durations, machines);
    }
}
