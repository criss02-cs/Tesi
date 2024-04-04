using Tesi.Solvers;
using Tesi.Solvers.Implementations;
using JobTask = Tesi.Solvers.Task;

namespace Tesi.Blazor.Server.Business;

/// <summary>
/// Classe di servizio che permette di usare tutti i solvers implementati.
/// Fa utilizzo del Design Pattern <see cref="https://refactoring.guru/design-patterns/strategy">Strategy</see> per poter scegliere
/// fra i diversi solvers, senza dover duplicare il codice.
/// </summary>
public class SolverService
{
    private readonly List<Job> _jobs =
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

    private int _numMachines;
    private int[] _allMachines = [];
    private int _horizon;
    private ISolver? _solver;

    #region CTORS

    public SolverService()
    {

    }
    public SolverService(ISolver? solver)
    {
        _solver = solver;
    }

    #endregion

    public void SetSolver(Solvers.Solvers solver)
    {
        _solver = solver switch
        {
            Solvers.Solvers.GOOGLE => new GoogleSolver(),
            Solvers.Solvers.IBM => new IbmSolver(),
            Solvers.Solvers.GUROBI => new GurobiSolver(),
            Solvers.Solvers.MICROSOFT => new MicrosoftSolver(),
            _ => _solver
        };
    }

    public SolverResult Solve()
    {
        CalculateData();
        return _solver?.Solve(_jobs, _horizon, _numMachines, _allMachines) ?? new SolverResult([], 0, "Solver not initialized");
    }

    private void CalculateData()
    {
        foreach (var task in _jobs.SelectMany(job => job.Tasks))
        {
            _numMachines = Math.Max(_numMachines, 1 + task.Machine);
        }
        _allMachines = Enumerable.Range(0, _numMachines).ToArray();
        foreach (var task in _jobs.SelectMany(job => job.Tasks))
        {
            _horizon += task.Duration;
        }
    }
}
