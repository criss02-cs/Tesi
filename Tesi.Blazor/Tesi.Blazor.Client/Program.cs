using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tesi.Blazor.Client;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using Syncfusion.Licensing;
using Tesi.Blazor.Client.Services;
using Tesi.Blazor.Client.Utils;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<SfDialogService>();
builder.Services.AddScoped<SolverService>();
builder.Services.AddSyncfusionBlazor();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
var license = await builder.GetSyncfusionLicense();
SyncfusionLicenseProvider.RegisterLicense(license);

await builder.Build().RunAsync();
