using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace PacifyAspire.ApiService
{
    public class MoodLoggerApi
    {
        private readonly HttpClient _httpClient;

        public MoodLoggerApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<String> CreateMoods(MoodLogs moodLogs)
        {

            try
            {

                var config = new ConfigurationBuilder()
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
.AddEnvironmentVariables()
.Build();

                var isAppTest = config["IsAppTest"];

                if (isAppTest == "false")
                {
                    await _httpClient.PostAsJsonAsync("http://localhost:7128/api/MoodCreator", moodLogs);
                    return "Success";
                }
                else
                {
                    await _httpClient.PostAsJsonAsync($"{config["AzFaMoodsCreator"]}", moodLogs);
                    return "Success";
                }
            } 
            catch (Exception ex)
            {
                return $"Exception caught: {ex.Message}";
            }
        }
    }

    public class MoodLogs
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Mood { get; set; } = string.Empty;
        public string MoodDescription { get; set; } = string.Empty;
        public string MoodIntensity { get; set; } = "Low";
        public DateTime MoodDate { get; set; } = DateTime.Today;
        public string UserId { get; set; }

    }


}


