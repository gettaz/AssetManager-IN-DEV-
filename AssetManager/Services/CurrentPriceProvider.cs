using AssetManager.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AssetManager.Services
{
    public class CurrentPriceProvider : ICurrentPriceProvider
    {
        private readonly HttpClient _client;

        public CurrentPriceProvider(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<string>> GetTickers()
        {
            var baseUrl = "https://api.finnhub.io/api/v1/stock/symbol?exchange=US&token=ch1hvi9r01qn6tg76npgch1hvi9r01qn6tg76nq0";

            try
            {
                HttpResponseMessage response = await _client.GetAsync(baseUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                var jsonArray = JsonConvert.DeserializeObject<JArray>(responseBody);
                var tickers = new List<string>();

                // Iterate through the array and extract the symbol property
                foreach (var item in jsonArray)
                {
                    string symbol = item["symbol"].ToString();
                    tickers.Add(symbol);
                }

                return tickers;
            }
            catch (HttpRequestException e)
            {
                // Handle potential network errors gracefully
                Console.WriteLine($"Error fetching stock tickers: {e.Message}");
                return new List<string>();
            }
        }
    }
}
