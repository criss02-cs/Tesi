using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesi.Blazor.Shared.Models;
using Tesi.Solvers;

namespace Tesi.Blazor.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ConfigController(IConfiguration config, IWebHostEnvironment webHostEnvironment) : ControllerBase
{
    [HttpGet, Route("SyncfusionLicense")]
    public IActionResult GetSyncfusionLicense()
    {
        var license = config["SyncfusionLicense"];
        return Ok(license);
    }

    [HttpGet, Route("GetSampleData")]
    public async Task<IActionResult> GetSampleData()
    {
        var sampleData = await System.IO.File.ReadAllTextAsync($"{webHostEnvironment.WebRootPath}/SampleData/sample-data.json");
        var data = JsonSerializer.Deserialize<List<Job>>(sampleData, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});
        var response = new ApiResponse<List<Job>>(data);
        return Ok(response);
    }

    [HttpGet, Route("GetLargeData")]
    public async Task<IActionResult> GetLargeData()
    {
        var largeData = await System.IO.File.ReadAllTextAsync($"{webHostEnvironment.WebRootPath}/SampleData/large-sample-data.json");
        var data = JsonSerializer.Deserialize<List<Job>>(largeData, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});
        var response = new ApiResponse<List<Job>>(data);
        return Ok(response);
    }
}
