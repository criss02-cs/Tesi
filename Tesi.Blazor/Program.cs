using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Tesi.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE4NjE0M0AzMjM1MmUzMDJlMzBvK3h2azNIck1nTjF6WmJZaGZKczRNc0pXQWZmMVNiNlVqakRzSjhORkE4PQ==");

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSyncfusionBlazor();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
await builder.Build().RunAsync();