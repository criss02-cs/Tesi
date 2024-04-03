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
        return Ok(service.SolveWithGoogle());
    }

    [HttpGet, Route("ibm")]
    public IActionResult Ibm()
    {
        return Ok(service.SolveWithIbm());
    }

    [HttpGet, Route("gurobi")]
    public IActionResult Gurobi()
    {
        return Ok(service.SolveWithGurobi());
    }
    [HttpGet, Route("microsoft")]
    public IActionResult Microsoft()
    {
        return Ok(service.SolveWithMicrosoft());
    }
}
