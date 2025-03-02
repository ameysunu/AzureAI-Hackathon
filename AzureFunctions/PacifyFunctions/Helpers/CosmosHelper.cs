using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using PacifyFunctions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacifyFunctions.Helpers
{
    public class CosmosHelper
    {
        private CosmosClient cosmosClient;
        private Container container;
        private ILogger _logger;

        string connectionString = Environment.GetEnvironmentVariable("COSMOSDB_CONN_STRING");
        string databaseName = Environment.GetEnvironmentVariable("COSMOSDB_DB_NAME");

        public CosmosHelper(ILogger _logger)
        {
            this._logger = _logger;
        }

        public void InitCosmosDb(String containerName)
        {
            cosmosClient = new CosmosClient(connectionString);
            container = cosmosClient.GetContainer(databaseName, containerName);
        }

        public async Task InsertMessage(String message, String imageUri)
        {
            try
            {
                var thought = new Thoughts
                {
                    Message = message,
                    TimeStamp = DateTime.Now,
                    Image = imageUri
                };

                await container.CreateItemAsync(thought, new PartitionKey(thought.Id));
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
