using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;
using PacifyFunctions.Models;
using System.Text.Json;

namespace PacifyFunctions
{
    public class GetThoughtsImages
    {
        private readonly ILogger<GetThoughtsImages> _logger;

        public GetThoughtsImages(ILogger<GetThoughtsImages> logger)
        {
            _logger = logger;
        }

        [Function("GetThoughtsImages")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.User, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {

                CosmosHelper cosmosHelper = new CosmosHelper(_logger);
                cosmosHelper.InitCosmosDb("positivitymessages");

                var thought = await cosmosHelper.GetThought();
                var jsonThought = JsonSerializer.Serialize<Thoughts>(thought);


                return new OkObjectResult(jsonThought);
            } 
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
