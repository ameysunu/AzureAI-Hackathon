using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace PacifyAspire.ApiService
{
    public class PostsApi
    {
        private readonly HttpClient _httpClient;

        public PostsApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<String> CreatePosts(CommInput postsData)
        {

            try
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();
                var isAppTest = config["IsAppTest"];

                if (isAppTest == "false")
                {
                    await _httpClient.PostAsJsonAsync("http://localhost:7128/api/AddToCommunityData", postsData);
                    return "Success";
                }
                else
                {
                    await _httpClient.PostAsJsonAsync($"{config["AzFaAddToCommunityData"]}", postsData);
                    return "Success";
                }
            }
            catch (Exception ex)
            {
                return $"Exception caught: {ex.Message}";
            }
        }
    }

    public class CommInput
    {
        public bool isComments { get; set; }
        public string postId { get; set; }
        public CommunityDataModel contents { get; set; }
    }


}


