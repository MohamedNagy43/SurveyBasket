using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using SurveyBasket.Api.ExtensionMethods.App;
using SurveyBasket.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseExceptionHandelMiddelware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
