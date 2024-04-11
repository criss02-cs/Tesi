using Tesi.Blazor.Shared.Models;

namespace Tesi.Blazor.Server.Middleware;

public class ErrorHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>(null, false, "Unauthorized access"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>(null, false, e.Message));
        }
    }
}