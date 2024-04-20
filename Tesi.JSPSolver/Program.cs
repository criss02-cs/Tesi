using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tesi.JSPSolver;
internal class Program
{
    static void Main(string[] args)
    {
        var solver = new Solver();

        string jsonData = @"[
  {
    ""jobId"": 0,
    ""tasks"": [
      {
        ""machine"": 0,
        ""duration"": 3,
        ""taskId"": 1
      },
      {
        ""machine"": 1,
        ""duration"": 2,
        ""taskId"": 2
      },
      {
        ""machine"": 2,
        ""duration"": 2,
        ""taskId"": 3
      }
    ]
  },
  {
    ""jobId"": 1,
    ""tasks"": [
      {
        ""machine"": 0,
        ""duration"": 2,
        ""taskId"": 1
      },
      {
        ""machine"": 2,
        ""duration"": 1,
        ""taskId"": 2
      },
      {
        ""machine"": 1,
        ""duration"": 4,
        ""taskId"": 3
      }
    ]
  },
  {
    ""jobId"": 2,
    ""tasks"": [
      {
        ""machine"": 1,
        ""duration"": 4,
        ""taskId"": 1
      },
      {
        ""machine"": 2,
        ""duration"": 3,
        ""taskId"": 2
      }
    ]
  }
]";

        List<Job>? jobs = JsonSerializer.Deserialize<List<Job>>(jsonData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

        if (jobs != null) solver.Jobs.AddRange(jobs);
        // Aggiungi alcuni lavori al risolutore

        var stopwatch = new Stopwatch();
        stopwatch.Start();


        // Esegui il metodo di Johnson
        solver.JohnsonMethod();

        stopwatch.Stop();
        Console.WriteLine($"Elapsed {stopwatch.ElapsedMilliseconds}");

        Console.ReadKey();
    }
}
