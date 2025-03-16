using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;
using PacifyFunctions.Models;
using System.Globalization;
using System.Text.Json;

namespace PacifyFunctions
{
    public class StatisticsGenerator
    {
        private readonly ILogger<StatisticsGenerator> _logger;

        public StatisticsGenerator(ILogger<StatisticsGenerator> logger)
        {
            _logger = logger;
        }

        [Function("StatisticsGenerator")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            MoodViewData data = JsonSerializer.Deserialize<MoodViewData>(requestBody);

            CosmosHelper cosmosHelper = new CosmosHelper(_logger);
            cosmosHelper.InitCosmosDb("moodLogs");

            var moods = await cosmosHelper.GetMoodsByUser(data.userId);

            if (moods.Count > 0)
            {
                try
                {
                    StatsModels statsModels = new StatsModels();
                    StatsEngine statsEngine = new StatsEngine(_logger, statsModels);

                    statsModels.frequentMoodIntensity = statsEngine.GetMostFrequentWords("intensity", moods);
                    statsModels.frequentMood = statsEngine.GetMostFrequentWords("mood", moods);

                    statsEngine.GetMoodIntensityStatistics(moods);
                    statsEngine.GetTimeBasedMoodTrends(moods);
                    statsEngine.DetectAnomalies(moods);

                    return new OkObjectResult(statsModels);
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult($"Exception caught: {ex.Message}");
                }
            }


            return new OkObjectResult("No data");
        }
    }
}
