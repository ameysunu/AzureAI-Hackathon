using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacifyFunctions.Helpers
{
    public class RedisHelper
    {
        public IDatabase _redisCache;
        private ILogger _logger;

        public RedisHelper(ILogger logger)
        {
            _logger = logger;
            InitRedis();
        }

        private void InitRedis()
        {
            var redisConnection = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("RedisConnectionString"));
            _redisCache = redisConnection.GetDatabase();
        }

        public async Task<String> GetCacheDataFromRedis(String cacheKey)
        {
            return await _redisCache.StringGetAsync(cacheKey);
        }

        public async Task SetDataToRedisCache(String cacheKey, String data)
        {
            await _redisCache.StringSetAsync(cacheKey, data);
        }
    }
}
