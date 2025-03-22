using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace PacifyAspire.ApiService
{
    public class GetCommunityData
    {
        private readonly HttpClient _httpClient;

        public GetCommunityData(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CommunityDataModel>> GetCommunityDataApi()
        {

            IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
            var isAppTest = config["IsAppTest"];
            HttpResponseMessage response;


            if (isAppTest == "false")
            {
                response = await _httpClient.GetAsync("http://localhost:7128/api/LoadCommunityData");
            }
            else
            {
                response = await _httpClient.GetAsync($"{config["AzFaLoadCommunityData"]}");
            }

            if (response.IsSuccessStatusCode)
            {
                var val = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(val))
                {
                    return new List<CommunityDataModel>();
                }

                return await response.Content.ReadFromJsonAsync<List<CommunityDataModel>>();
            }

            return null;
        }

    }

    public class CommunityDataModel
    {
        public String id { get; set; } = Guid.NewGuid().ToString();
        public String userId { get; set; }
        public String userName { get; set; }
        public DateTime createdOn { get; set; }
        public String contents { get; set; }
        public int upVotes { get; set; }
        public List<CommunityDataModel> comments { get; set; }
    }

}


