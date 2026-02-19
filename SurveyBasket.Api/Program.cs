using SurveyBasket.Api;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDependencies();

var app = builder.Build();

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
