using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;

namespace PacifyFunctions
{
    public class ThoughtsAndImagesGenerator
    {
        private readonly ILogger<ThoughtsAndImagesGenerator> _logger;

        public ThoughtsAndImagesGenerator(ILogger<ThoughtsAndImagesGenerator> logger)
        {
            _logger = logger;
        }

        [Function("ThoughtsAndImagesGenerator")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            try
            {
                //Get data from OpenAI
                //Write into CosmosDB
                //Convert Function to Timer to run every 6 hours

                _logger.LogInformation("ThoughtsAndImagesGeneratorfunction processed a request.");

                OpenAIHelper openAIHelper = new OpenAIHelper(_logger);
                await openAIHelper.SendTextMessagePrompt("You're a positive thoughts reflective machine", "Generate a positive thought for the day");

                return new OkObjectResult("Welcome to Azure Functions!");

            } catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
