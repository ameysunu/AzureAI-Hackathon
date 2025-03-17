using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace PacifyAspire.ApiService
{
    public class StatsApi
    {
        private readonly HttpClient _httpClient;

        public StatsApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<StatsModels>> GetStatsByUser(MoodViewData mData)
        {

            IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            var isAppTest = config["IsAppTest"];
            HttpResponseMessage response;


            if (isAppTest == "false")
            {
                response = await _httpClient.PostAsJsonAsync("http://localhost:7128/api/GetStatsByUser", value: mData);
            }
            else
            {
                response = await _httpClient.PostAsJsonAsync(config["AzFaGetStatsByUser"], value: mData);
            }

            if (response.IsSuccessStatusCode)
            {
                var val = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(val))
                {
                    return new List<StatsModels>();
                }

                return await response.Content.ReadFromJsonAsync<List<StatsModels>>();
            }

            return null;
        }

        public class StatsModels
        {
            public string frequentMoodIntensity { get; set; }
            public string frequentMood { get; set; }
            public Dictionary<string, double> moodIntensityDistribution { get; set; }
            public double moodIntensityVariance { get; set; }
            public List<string> dailyMoodChanges { get; set; }
            public Dictionary<string, string> commonMoodsPerTimeframe { get; set; }
            public Dictionary<string, double> longestMoodStreak { get; set; }
            public List<string> suddenMoodShift { get; set; }
            public List<string> unusualHighIntensityMoodPattern { get; set; }
            public string userId { get; set; }
            public string id { get; set; }
        }

    }
}


