using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Syncfusion.Blazor.Popups;
using Tesi.Blazor.Shared.Models;
using Tesi.Solvers;

namespace Tesi.Blazor.Client.Services;

public class SolverService(HttpClient http, SfDialogService dialogService)
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true, IncludeFields = true};
    public async Task<SolverResult?> Solve(string solver)
    {
        var response = await http.GetAsync($"Or/{solver}");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<SolverResult>>(content, Options);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return result?.Result ?? new SolverResult([], 0, "");
            case HttpStatusCode.InternalServerError:
                {
                    await dialogService.AlertAsync(result?.Message, "Errore");
                    return new SolverResult([], 0, "");
                }
            default:
                return new SolverResult([], 0, "");
        }
    }
}