using Tesi.Solvers;
using Tesi.Solvers.Implementations;
using JobTask = Tesi.Solvers.Task;

namespace Tesi.Blazor.Server.Business;

public class SolverService
{
    private List<Job> _jobs =
    [
        new Job(0, [
            new JobTask(0, 3, 1),
            new JobTask(1, 2, 2),
            new JobTask(2, 2, 3),
        ]),
        new Job(1, [
            new JobTask(0, 2, 1),
            new JobTask(2, 1, 2),
            new JobTask(1, 4, 3),
        ]),
        new Job(2, [
            new JobTask(1, 4, 1),
            new JobTask(2, 3, 2),
        ]),
    ];

    private int _numMachines = 0;
    private int[] _allMachines = [];
    private int _horizon = 0;
    public SolverResult SolveWithGoogle()
    {
        CalculateData();
        var solver = new GoogleSolver();
        return solver.Solve(_jobs, _horizon, _numMachines, _allMachines);
    }

    public SolverResult SolveWithIbm()
    {
        CalculateData();
        var solver = new IbmSolver();
        return solver.Solve(_jobs, _horizon, _numMachines, _allMachines);
    }

    public SolverResult SolveWithGurobi()
    {
        CalculateData();
        var solver = new GurobiSolver();
        return solver.Solve(_jobs, _horizon, _numMachines, _allMachines);
    }
    public SolverResult SolveWithMicrosoft()
    {
        CalculateData();
        var solver = new MicrosoftSolver();
        return solver.Solve(_jobs, _horizon, _numMachines, _allMachines);
    }
    private void CalculateData()
    {
        foreach (var job in _jobs)
        {
            foreach (var task in job.Tasks)
            {
                _numMachines = Math.Max(_numMachines, 1 + task.Machine);
            }
        }
        _allMachines = Enumerable.Range(0, _numMachines).ToArray();
        foreach (var job in _jobs)
        {
            foreach (var task in job.Tasks)
            {
                _horizon += task.Duration;
            }
        }
    }
}
