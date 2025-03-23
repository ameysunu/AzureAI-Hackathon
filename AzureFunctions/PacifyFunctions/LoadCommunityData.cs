using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Helpers;
using PacifyFunctions.Models;
using System.Text.Json;

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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                RedisHelper redisHelper = new RedisHelper(_logger);
                var cacheData = await redisHelper.GetCacheDataFromRedis("communityData");

                if (cacheData != null)
                {
                    return new OkObjectResult(JsonSerializer.Deserialize<CommunityDataModel>(cacheData));
                }
                else
                {
                    CosmosHelper cosmosHelper = new CosmosHelper(_logger);
                    cosmosHelper.InitCosmosDb("communityData");

                    var data = await cosmosHelper.GetCommunityData();
                    await redisHelper._redisCache.StringSetAsync("communityData", JsonSerializer.Serialize<List<CommunityDataModel>>(data));

                    return new OkObjectResult(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
