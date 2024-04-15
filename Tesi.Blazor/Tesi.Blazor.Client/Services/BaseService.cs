using System.Text.Json;
using Syncfusion.Blazor.Popups;

namespace Tesi.Blazor.Client.Services;

public abstract class BaseService(HttpClient http, SfDialogService dialogService)
{
    protected static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true, IncludeFields = true };
    protected readonly HttpClient Http = http;
    protected readonly SfDialogService DialogService = dialogService;
}
