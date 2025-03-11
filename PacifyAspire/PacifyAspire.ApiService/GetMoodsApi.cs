using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace PacifyAspire.ApiService
{
    public class GetMoodsApi
    {
        private readonly HttpClient _httpClient;

        public GetMoodsApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MoodLogs>> GetMoodsByUser(MoodViewData mData)
        {

           IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
           var isAppTest = config["IsAppTest"];
            HttpResponseMessage response;


            if (isAppTest == "false")
            {
                response = await _httpClient.PostAsJsonAsync("http://localhost:7128/api/GetMoodsByUser", value: mData);
            }
            else
            {
                response = await _httpClient.PostAsJsonAsync($"{config["AzFaGetMoodsByUser"]}", value: mData);
            }

            if (response.IsSuccessStatusCode)
            {
                var val = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(val))
                {
                    return new List<MoodLogs>();
                }

                return await response.Content.ReadFromJsonAsync<List<MoodLogs>>();
            }

            return null;
        }

    }

    public class MoodViewData
    {
        public string userId { get; set; }
    }
}


