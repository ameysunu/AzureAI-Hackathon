using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;
using PacifyFunctions.Models;
using System.Text.Json;

namespace PacifyFunctions
{
    public class GetStatsByUser
    {
        private readonly ILogger<GetStatsByUser> _logger;

        public GetStatsByUser(ILogger<GetStatsByUser> logger)
        {
            _logger = logger;
        }

        [Function("GetStatsByUser")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                MoodViewData data = JsonSerializer.Deserialize<MoodViewData>(requestBody);

                CosmosHelper cosmosHelper = new CosmosHelper(_logger);
                cosmosHelper.InitCosmosDb("statsData");

                var statsData = await cosmosHelper.GetStatsByUser(data.userId);

                return new OkObjectResult(statsData);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
