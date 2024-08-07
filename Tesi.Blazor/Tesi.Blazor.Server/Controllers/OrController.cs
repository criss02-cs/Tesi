﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesi.Blazor.Server.Business;
using Tesi.Blazor.Server.Exceptions;
using Tesi.Blazor.Shared.ExtensionMethods;
using Tesi.Blazor.Shared.Models;
using Tesi.Solvers;

namespace Tesi.Blazor.Server.Controllers;
[Route("[controller]")]
[ApiController]
public class OrController(SolverService service) : ControllerBase
{
    [HttpPost, Route("{solver}")]
    public IActionResult Solve([FromBody] List<Job> data, string solver)
    {
        if (!Enum.TryParse<Solvers.Solvers>(solver.ToTitleCase(), out var solverType))
        {
            throw new SolverNotFoundException($"Solver {solver} non trovato");
        }
        service.SetSolver(solverType);
        return Ok(service.Solve(data));
    }

    [HttpPost, Route("analysis/{numOfExecutions:int}")]
    public IActionResult MakeAnalysis(int numOfExecutions, [FromBody] List<Job> data)
    {
        var result = new List<Analysis>();
        var solvers = Enum.GetNames<Solvers.Solvers>();
        foreach (var solver in solvers)
        {
            if (!Enum.TryParse<Solvers.Solvers>(solver.ToTitleCase(), out var solverType))
            {
                throw new SolverNotFoundException($"Solver {solver} non trovato");
            }
            service.SetSolver(solverType);
            var results = new List<ResultAnalysis>();
            for (var i = 0; i < numOfExecutions; i++)
            {
                try
                {
                    var solved = service.Solve(data);
                    results.Add(new ResultAnalysis(i + 1, solved.Result?.ElapsedMilliseconds ?? 0));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    results.Add(new ResultAnalysis(i + 1, 0));
                }
            }
            var analysis = new Analysis(solverType, results.Average(r => r.Durations), results);
            result.Add(analysis);
        }

        return Ok(new ApiResponse<List<Analysis>>(result));
    }
}

