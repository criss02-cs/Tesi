using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Syncfusion.Blazor.Popups;
using Tesi.Blazor.Shared.Models;
using Tesi.Solvers;

namespace Tesi.Blazor.Client.Services;

public class SolverService(HttpClient http, SfDialogService dialogService)
{
    public async Task<SolverResult?> Solve(string solver)
    {
        var response = await http.GetAsync($"Or/{solver}");
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<SolverResult>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }
            case HttpStatusCode.InternalServerError:
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ApiResponse<object>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                await dialogService.AlertAsync(result?.Message, "Errore");
                return new SolverResult([], 0, "");
            }
            default:
                return new SolverResult([], 0, "");
        }
    }
}