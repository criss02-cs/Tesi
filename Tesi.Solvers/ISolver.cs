using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi.Solvers;
public interface ISolver
{
    protected int Horizon { get; set; }
    protected int NumMachines { get; set; }
    protected int[] AllMachines { get; set; }
    public SolverResult Solve(List<Job> jobs);

    public void CalculateData(List<Job> jobs)
    {
        foreach (var task in jobs.SelectMany(job => job.Tasks))
        {
            NumMachines = Math.Max(NumMachines, 1 + task.Machine);
        }

        AllMachines = Enumerable.Range(0, NumMachines).ToArray();
        foreach (var task in jobs.SelectMany(job => job.Tasks))
        {
            Horizon += task.Duration;
        }
    }
}
