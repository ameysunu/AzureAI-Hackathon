using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;
using PacifyFunctions.Models;
using System.Text.Json;

namespace PacifyFunctions
{
    public class GetMoodsByUser
    {
        private readonly ILogger<GetMoodsByUser> _logger;

        public GetMoodsByUser(ILogger<GetMoodsByUser> logger)
        {
            _logger = logger;
        }

        [Function("GetMoodsByUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                MoodViewData data = JsonSerializer.Deserialize<MoodViewData>(requestBody);

                CosmosHelper cosmosHelper = new CosmosHelper(_logger);
                cosmosHelper.InitCosmosDb("moodLogs");
                var moods = await cosmosHelper.GetMoodsByUser(data.userId);

                _logger.LogInformation("C# HTTP trigger function processed a request.");
                return new OkObjectResult(moods); 
            } 
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                return new OkObjectResult(ex.Message)
                {
                    StatusCode = 500
                };
            }
        }
    }
}
