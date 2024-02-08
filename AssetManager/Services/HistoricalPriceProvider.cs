using AssetManager.DTO;
using AssetManager.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class HistoricalPriceProvider : IHistoricalPriceProvider
{
    private readonly HttpClient _client;

    public HistoricalPriceProvider(HttpClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<TimelineDataItem>> GetHistoricalPriceAsync(string symbol, string fromDate, string toDate)
    {
        var baseUrl = $"https://financialmodelingprep.com/api/v3/historical-price-full/{symbol}";
        var urlWithParams = $"{baseUrl}?from={fromDate}&to={toDate}&apikey=aUV5EG7zNmn6kvp6qpi3qjnh5q0mgltO";

        try
        {
            HttpResponseMessage response = await _client.GetAsync(urlWithParams);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var prices = new List<TimelineDataItem>();
            var jsobjon = (JObject)JsonConvert.DeserializeObject(responseBody);

            var pricesArray = jsobjon["historical"];

            foreach (var item in pricesArray)
            {
                prices.Add(new TimelineDataItem
                {
                    Date = item["date"].ToObject<DateTime>(),
                    Price = item["close"].ToObject<double>()
                });
            }

            return prices;
        }
        catch (HttpRequestException e)
        {
            throw new Exception("Error fetching data: " + e.Message);
        }
    }

    public async Task<IEnumerable<string>> GetTickers()
    {
        var baseUrl = "https://financialmodelingprep.com/api/v3/stock/list?apikey=aUV5EG7zNmn6kvp6qpi3qjnh5q0mgltO";

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
