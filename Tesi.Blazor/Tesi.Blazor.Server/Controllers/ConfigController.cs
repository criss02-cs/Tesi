using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tesi.Blazor.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ConfigController(IConfiguration config) : ControllerBase
{
    [HttpGet, Route("SyncfusionLicense")]
    public IActionResult GetSyncfusionLicense()
    {
        var license = config["SyncfusionLicense"];
        return Ok(license);
    }
}
