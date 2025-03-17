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
        public async Task Run([CosmosDBTrigger(
            databaseName: "azureaihack",
            containerName: "moodLogs",
            Connection = "COSMOSDB_CONN_STRING",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = true)] IReadOnlyList<MoodLogs> input)
        {
            _logger.LogInformation("CosmosDb trigger function processed a request.");

            if (input != null && input.Count > 0)
            {

                CosmosHelper cosmosHelper = new CosmosHelper(_logger);
                cosmosHelper.InitCosmosDb("moodLogs");

                var moods = await cosmosHelper.GetMoodsByUser(input[0].userId);

                if (moods.Count > 0)
                {
                    try
                    {
                        StatsModels statsModels = new StatsModels();
                        statsModels.userId = input[0].userId;

                        StatsEngine statsEngine = new StatsEngine(_logger, statsModels);
                        OpenAIHelper openAIHelper = new OpenAIHelper(_logger);

                        statsModels.frequentMoodIntensity = statsEngine.GetMostFrequentWords("intensity", moods);
                        statsModels.frequentMood = statsEngine.GetMostFrequentWords("mood", moods);

                        statsEngine.GetMoodIntensityStatistics(moods);
                        statsEngine.GetTimeBasedMoodTrends(moods);
                        statsEngine.DetectAnomalies(moods);

                        await statsEngine.GenerateAdvisoryMoodDescriptionNotes(moods, openAIHelper);

                        await cosmosHelper.UpsertStatsData(statsModels, "statsData");

                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Exception caught: {ex.Message}");
                    }
                }
            }


            //return new OkObjectResult("No data");
        }
    }
}
