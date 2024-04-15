using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Syncfusion.Blazor.Popups;
using Tesi.Blazor.Shared.Models;
using Tesi.Solvers;

namespace Tesi.Blazor.Client.Services;

public class SolverService(HttpClient http, SfDialogService dialogService) : BaseService(http, dialogService)
{
    public async Task<SolverResult?> Solve(string solver, List<Job> jobs)
    {
        var response = await Http.PostAsJsonAsync($"Or/{solver}", jobs);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<SolverResult>>(content, Options);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return result?.Result ?? new SolverResult([], 0, "");
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.BadRequest:
                {
                    await DialogService.AlertAsync(result?.Message, "Errore");
                    return new SolverResult([], 0, "");
                }
            default:
                return new SolverResult([], 0, "");
        }
    }

    public async Task<List<Analysis>> MakeAnalysis(int numOfExecutions, List<Job> jobs)
    {
        var response = await Http.PostAsJsonAsync($"Or/analysis/{numOfExecutions}", jobs);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<List<Analysis>>>(content, Options);
        return result?.Result ?? new List<Analysis>();
    }
}