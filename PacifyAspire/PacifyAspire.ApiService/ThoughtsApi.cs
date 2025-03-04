namespace PacifyAspire.ApiService
{
    public class ThoughtsApi
    {
        private readonly HttpClient _httpClient;

        public ThoughtsApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetDataAsync()
        {
            var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

            var isAppTest = config["IsAppTest"];

            if (isAppTest == "false")
            {
                return await _httpClient.GetStringAsync("http://localhost:7128/api/GetThoughtsImages");
            } else
            {
                return await _httpClient.GetStringAsync($"{config["AzFaGetThoughtsImages"]}");
            }
        }
    }

}
