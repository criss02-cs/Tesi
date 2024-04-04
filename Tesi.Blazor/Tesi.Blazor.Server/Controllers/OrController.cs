using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesi.Blazor.Server.Business;

namespace Tesi.Blazor.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OrController(SolverService service) : ControllerBase
{
    [HttpGet, Route("google")]
    public IActionResult Google()
    {
        service.SetSolver(Solvers.Solvers.GOOGLE);
        return Ok(service.Solve());
    }

    [HttpGet, Route("ibm")]
    public IActionResult Ibm()
    {
        service.SetSolver(Solvers.Solvers.IBM);
        return Ok(service.Solve());
    }

    [HttpGet, Route("gurobi")]
    public IActionResult Gurobi()
    {
        service.SetSolver(Solvers.Solvers.GUROBI);
        return Ok(service.Solve());
    }
    [HttpGet, Route("microsoft")]
    public IActionResult Microsoft()
    {
        service.SetSolver(Solvers.Solvers.MICROSOFT);
        return Ok(service.Solve());
    }
}
