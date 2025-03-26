using Microsoft.AspNetCore.Mvc;
using PacifyAspire.ApiService;
using System.Text.Json;
using static PacifyAspire.ApiService.StatsApi;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient<ThoughtsApi>();
builder.Services.AddHttpClient<MoodLoggerApi>();
builder.Services.AddHttpClient<GetMoodsApi>();
builder.Services.AddHttpClient<StatsApi>();
builder.Services.AddHttpClient<GetCommunityData>();
builder.Services.AddHttpClient<PostsApi>();
builder.Services.AddHttpClient<GeneratePdf>();

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

app.MapGet("/getmoodbyuser", async (HttpContext context, [FromServices] GetMoodsApi getMoodsApi) =>
{
    var userId = context.Request.Query["userId"].ToString();

    if (string.IsNullOrEmpty(userId))
    {
        return Results.BadRequest("User ID is null or empty");
    }

    try
    {
        List<MoodLogs> result = await getMoodsApi.GetMoodsByUser(new MoodViewData { userId = userId });
        return Results.Ok(result);
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem($"Failed to fetch moods: {ex.Message}");
    }
});

app.MapGet("/getstatsbyuser", async (HttpContext context, [FromServices] StatsApi statsApi) =>
{
    var userId = context.Request.Query["userId"].ToString();

    if (string.IsNullOrEmpty(userId))
    {
        return Results.BadRequest("User ID is null or empty");
    }

    try
    {
        List<StatsModels> result = await statsApi.GetStatsByUser(new MoodViewData { userId = userId });
        return Results.Ok(result);
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem($"Failed to fetch moods: {ex.Message}");
    }
});

app.MapGet("/getcommdata", async (GetCommunityData getCommunityData) =>
{
    return await getCommunityData.GetCommunityDataApi();
});

app.MapPost("/writeposts", async (HttpContext context, [FromServices] PostsApi postApi) =>
{
    try
    {
        var commInput = await context.Request.ReadFromJsonAsync<CommInput>();
        if (commInput == null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid request data");
            return;
        }


        await postApi.CreatePosts(commInput);
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("Post created!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Internal Server Error");
    }
});

app.MapGet("/generatepdf", async (GeneratePdf generatePdf) => 
{
    return await generatePdf.GenerateMoodsPdf();
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
