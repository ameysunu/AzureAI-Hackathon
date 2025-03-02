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
            return await _httpClient.GetStringAsync("http://localhost:7128/api/GetThoughtsImages");
        }
    }

}
