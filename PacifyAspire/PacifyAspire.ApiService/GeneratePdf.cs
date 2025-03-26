namespace PacifyAspire.ApiService
{
    public class GeneratePdf
    {
        private readonly HttpClient _httpClient;

        public GeneratePdf(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GenerateMoodsPdf()
        {
            var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

            var isAppTest = config["IsAppTest"];

            if (isAppTest == "false")
            {
                return "Hello test world";
            }
            else
            {
                return "Hello test world";
            }
        }
    }

}
