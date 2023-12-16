using AssetManager.DTO;
using AssetManager.Interfaces;
using Newtonsoft.Json.Linq;

public class PriceProvider : IPriceProvider
{
    private readonly HttpClient _client;

    public PriceProvider(HttpClient client)
    {
        _client = client;
    }

    public async Task<TimelineSummaryDto> GetHistoricalPriceAsync(string symbol, string fromDate, string toDate)
    {
        var baseUrl = $"https://financialmodelingprep.com/api/v3/historical-price-full/{symbol}";
        var urlWithParams = $"{baseUrl}?from={fromDate}&to={toDate}&apikey=aUV5EG7zNmn6kvp6qpi3qjnh5q0mgltO";

        try
        {
            HttpResponseMessage response = await _client.GetAsync(urlWithParams);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var jsonObject = JObject.Parse(responseBody);
            var historicalData = jsonObject["historical"].ToObject<List<JObject>>();

            var prices = new List<TimelineDataItem>();
            foreach (var item in historicalData)
            {
                prices.Add(new TimelineDataItem
                {
                    Date = item["date"].ToObject<DateTime>(),
                    Price = item["close"].ToObject<double>()
                });
            }

            return new TimelineSummaryDto
            {
                Name = symbol,
                Prices = prices
            };
        }
        catch (HttpRequestException e)
        {
            throw new Exception("Error fetching data: " + e.Message);
        }
    }
}
