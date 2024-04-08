using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Tesi.Blazor.Client.Utils;

public static class HostBuilderExtensions
{
    public static async Task<string> GetSyncfusionLicense(this WebAssemblyHostBuilder builder)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        };
        var license = await httpClient.GetStringAsync("api/Config/SyncfusionLicense");
        return license;
    }
}
