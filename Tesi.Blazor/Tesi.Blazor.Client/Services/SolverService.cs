using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Syncfusion.Blazor.Popups;
using Tesi.Blazor.Shared.Models;
using Tesi.Solvers;

namespace Tesi.Blazor.Client.Services;

public class SolverService(HttpClient http, SfDialogService dialogService) : BaseService(http, dialogService)
{
    public async Task<SolverResult?> Solve(string solver)
    {
        var response = await Http.GetAsync($"Or/{solver}");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<SolverResult>>(content, Options);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return result?.Result ?? new SolverResult([], 0, "");
            case HttpStatusCode.InternalServerError:
                {
                    await DialogService.AlertAsync(result?.Message, "Errore");
                    return new SolverResult([], 0, "");
                }
            default:
                return new SolverResult([], 0, "");
        }
    }
}