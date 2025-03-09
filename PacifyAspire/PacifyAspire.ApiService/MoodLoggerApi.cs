using Microsoft.AspNetCore.Mvc;

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
                return "Mood Logs api called!";
            } 
            catch (Exception ex)
            {
                return $"Exception caught: {ex.Message}";
            }
        }
    }

    public class MoodLogs
    {
        public string Mood { get; set; } = string.Empty;
        public string MoodDescription { get; set; } = string.Empty;
        public string MoodIntensity { get; set; } = "Low";
        public DateTime MoodDate { get; set; } = DateTime.Today;

    }


}


