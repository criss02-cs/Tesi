using System.Net;
using System.Text.Json;
using Syncfusion.Blazor.Popups;
using Tesi.Blazor.Shared.Models;
using Tesi.Solvers;

namespace Tesi.Blazor.Client.Services;

public class ConfigService(HttpClient http, SfDialogService service) : BaseService(http, service)
{
    public async Task<List<Job>> LoadSampleData()
    {
        var response = await Http.GetAsync($"api/Config/GetLargeData");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<List<Job>>>(content, Options);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return result?.Result ?? [];
            case HttpStatusCode.InternalServerError:
            {
                await DialogService.AlertAsync(result?.Message, "Errore");
                return [];
            }
            default:
                return [];
        }
    }
}
