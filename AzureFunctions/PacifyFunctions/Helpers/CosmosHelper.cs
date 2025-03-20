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

        public async Task InsertMoods(MoodLogs moodLogs)
        {
            try
            {
                await container.CreateItemAsync(moodLogs, new PartitionKey(moodLogs.id));
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<MoodLogs>> GetMoodsByUser(String userId)
        {
            try
            {
                List<MoodLogs> moodLogs = new List<MoodLogs>();
                using FeedIterator<MoodLogs> moodLogFromCosmos = container.GetItemQueryIterator<MoodLogs>(
                    queryText: $"SELECT * FROM c WHERE c.userId = '{userId}' "
                );

                while (moodLogFromCosmos.HasMoreResults)
                {
                    FeedResponse<MoodLogs> response = await moodLogFromCosmos.ReadNextAsync();

                    foreach (MoodLogs item in response)
                    {
                        moodLogs.Add(item);
                    }
                }

                return moodLogs;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<StatsModels>> GetStatsByUser(String userId)
        {
            try
            {
                List<StatsModels> statsData = new List<StatsModels>();

                using FeedIterator<StatsModels> statsDataFromCosmos = container.GetItemQueryIterator<StatsModels>(
                    queryText: $"SELECT * FROM c WHERE c.userId = '{userId}' "
                );

                while (statsDataFromCosmos.HasMoreResults)
                {
                    FeedResponse<StatsModels> response = await statsDataFromCosmos.ReadNextAsync();

                    foreach (StatsModels item in response)
                    {
                        statsData.Add(item);
                    }
                }

                return statsData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Thoughts> GetThought()
        {
            try
            {
                _logger.LogInformation("Retrieving thoughts");

                QueryDefinition query = new QueryDefinition("SELECT * FROM c");
                using FeedIterator<Thoughts> resultSet = container.GetItemQueryIterator<Thoughts>(query);

                if (resultSet.HasMoreResults)
                {
                    FeedResponse<Thoughts> response = await resultSet.ReadNextAsync();
                    if (response != null)
                    {
                        return response.FirstOrDefault();
                    }
                }
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }

            return null;
        }

        public async Task<byte[]> ConvertDalleUriToImageBytes(String uri)
        {
            using HttpClient client = new HttpClient();
            return await client.GetByteArrayAsync(uri);
        }

        public async Task UpsertStatsData(StatsModels statsData, String containerName)
        {
            try
            {
                var statsContainer = cosmosClient.GetContainer(databaseName, containerName);
                statsData.id = statsData.userId;

                await statsContainer.UpsertItemAsync(statsData, new PartitionKey(statsData.userId));
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CommunityDataModel>> GetCommunityData()
        {
            try
            {
                List<CommunityDataModel> communityData = new List<CommunityDataModel>();
                using FeedIterator<CommunityDataModel> communityDataFromCosmos = container.GetItemQueryIterator<CommunityDataModel>(
                    queryText: $"SELECT * FROM c"
                );

                while (communityDataFromCosmos.HasMoreResults)
                {
                    FeedResponse<CommunityDataModel> response = await communityDataFromCosmos.ReadNextAsync();

                    foreach (CommunityDataModel item in response)
                    {
                        communityData.Add(item);
                    }
                }

                return communityData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task InsertComments(CommunityDataModel data, string postId)
        {
            try
            {
                ItemResponse<CommunityDataModel> response = await container.ReadItemAsync<CommunityDataModel>(
                    postId, new PartitionKey(postId)
                );

                CommunityDataModel post = response.Resource;

                if (post == null)
                {
                    _logger.LogError($"Post with ID {postId} not found.");
                    return;
                }

                if (post.comments == null)
                {
                    post.comments = new List<CommunityDataModel>();
                }

                post.comments.Add(data);


                var upsertResponse = await container.UpsertItemAsync(post, new PartitionKey(postId));
                _logger.LogInformation($"Upserted Item Status: {upsertResponse.StatusCode}");

            }
            catch (CosmosException ex)
            {
                _logger.LogError($"CosmosDB Error: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }


        public async Task InsertPost(CommunityDataModel data)
        {
            try
            {
                await container.CreateItemAsync(data, new PartitionKey(data.id));
            }
            catch (CosmosException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
