using Microsoft.AspNetCore.Mvc;
using PacifyAspire.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<ThoughtsApi>();
builder.Services.AddHttpClient<MoodLoggerApi>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/thoughts", async (ThoughtsApi thoughtsApi) =>
{
    return await thoughtsApi.GetDataAsync(); 
});


app.MapPost("/createmoods", async (MoodLogs moodLogs, [FromServices] MoodLoggerApi moodLoggerApi) =>
{
    if (moodLogs == null)
    {
        return Results.BadRequest("Invalid mood data.");
    }


    var result = await moodLoggerApi.CreateMoods(moodLogs);

    if (result.Contains("Exception"))
    {
        return Results.InternalServerError(result);
    }

    return Results.Ok(result);
});


//BOILER PLATE CODE BELOW
string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
