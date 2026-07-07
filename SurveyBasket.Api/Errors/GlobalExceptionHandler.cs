using Microsoft.AspNetCore.Diagnostics;

namespace SurveyBasket.Api.Errors;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        _logger.LogError(exception, "An unhandled exception occurred {Message}", exception.Message);

        var ProblemDetails = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#name-500-internal-server-error",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(ProblemDetails, cancellationToken);
        return true;
    }
}
