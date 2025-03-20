using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;
using PacifyFunctions.Models;
using System.Text.Json;

namespace PacifyFunctions
{
    public class AddToCommunityData
    {
        private readonly ILogger<AddToCommunityData> _logger;

        public AddToCommunityData(ILogger<AddToCommunityData> logger)
        {
            _logger = logger;
        }

        [Function("AddToCommunityData")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            try
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                var request = await new StreamReader(req.Body).ReadToEndAsync();
                var reqJson = JsonSerializer.Deserialize<CommInput>(request);

                CosmosHelper cosmosHelper = new CosmosHelper(_logger);
                cosmosHelper.InitCosmosDb("communityData");


                if (reqJson.isComments)
                {
                    _logger.LogInformation("Type is comments");
                    await cosmosHelper.InsertComments(reqJson.contents, reqJson.postId);
                   
                    
                } else
                {
                    _logger.LogInformation("Type is post");
                    await cosmosHelper.InsertPost(reqJson.contents);
                }

                    return new OkObjectResult("Welcome to Azure Functions!");
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
