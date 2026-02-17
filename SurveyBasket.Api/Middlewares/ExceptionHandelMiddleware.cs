using Microsoft.AspNetCore.Diagnostics;

namespace SurveyBasket.Api.Middlewares;

public class ExceptionHandelMiddleware(RequestDelegate next,ILogger<ExceptionHandelMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandelMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred. Please try again later." });
        }
    }
}
