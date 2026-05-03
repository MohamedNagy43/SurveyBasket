using Hangfire;
using HangfireBasicAuthenticationFilter;
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
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

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

RecurringJob.AddOrUpdate("SendNewPollNotificationAsync", () => NotificationService.SendNewPollNotificationAsync(null),Cron.Daily());

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
