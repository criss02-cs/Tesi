using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tesi.Solvers;
public interface ISolver
{
    public SolverResult Solve(List<Job> jobs, int horizon, int numMachines, int[] allMachines);
}
