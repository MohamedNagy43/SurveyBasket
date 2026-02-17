using SurveyBasket.Api.Middlewares;

namespace SurveyBasket.Api.ExtensionMethods.App;

public static class CustomeMiddelwaresExtensions
{
    public static IApplicationBuilder UseExceptionHandelMiddelware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandelMiddleware>();
        return app;
    }
}
