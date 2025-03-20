using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;

namespace PacifyFunctions
{
    public class LoadCommunityData
    {
        private readonly ILogger<LoadCommunityData> _logger;

        public LoadCommunityData(ILogger<LoadCommunityData> logger)
        {
            _logger = logger;
        }

        [Function("LoadCommunityData")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                CosmosHelper cosmosHelper = new CosmosHelper(_logger);
                cosmosHelper.InitCosmosDb("communityData");

                var data = await cosmosHelper.GetCommunityData();

                return new OkObjectResult(data);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
