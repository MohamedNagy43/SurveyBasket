using Hangfire;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using SurveyBasket.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, loggerConfiguration)
    => loggerConfiguration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
        options.InjectStylesheet("/swagger-dark.css");
    });
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
           User = app.Configuration.GetValue<string>("HangfireSettings:UserName"),
           Pass = app.Configuration["HangfireSettings:Password"]
        }
    ],
    DashboardTitle = "SurveyBasket Jobs",
    IsReadOnlyFunc = (context) => true
});

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var Scope = scopeFactory.CreateScope();
var NotificationService = Scope.ServiceProvider.GetRequiredService<INotificationService>();

RecurringJob.AddOrUpdate("SendNewPollNotificationAsync", () => NotificationService.SendNewPollNotificationAsync(null), Cron.Daily());

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
