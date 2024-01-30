using AssetManager.DTO;
using AssetManager.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PriceProvider : IPriceProvider
{
    private readonly HttpClient _client;

    public PriceProvider(HttpClient client)
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
}
