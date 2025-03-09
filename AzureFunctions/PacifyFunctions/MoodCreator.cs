using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;
using PacifyFunctions.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace PacifyFunctions
{
    public class MoodCreator
    {
        private readonly ILogger<MoodCreator> _logger;

        public MoodCreator(ILogger<MoodCreator> logger)
        {
            _logger = logger;
        }

        [Function("MoodCreator")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                if (requestBody != null)
                {
                    MoodLogs moodLogs = JsonSerializer.Deserialize<MoodLogs>(requestBody);
                    _logger.LogInformation("Request body successfully deserialized");

                    CosmosHelper cosmosHelper = new CosmosHelper(_logger);
                    cosmosHelper.InitCosmosDb("moodLogs");

                    await cosmosHelper.InsertMoods(moodLogs);

                    return new OkObjectResult("Added mood to CosmosDb successfully");
                } 
                else
                {
                    return new BadRequestObjectResult("Request is null");
                }
            } 
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
