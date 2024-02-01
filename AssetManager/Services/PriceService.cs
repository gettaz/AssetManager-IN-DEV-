using AssetManager.DTO;
using AssetManager.Interfaces;

public class PriceService : IPriceService
{
    private readonly IPriceProvider _pricesProvider;
    private readonly IAssetRepository _assetRepository;
    private readonly ICategoryRepository _categoryRepository;

    public PriceService(IPriceProvider pricesProvider, IAssetRepository assetRepository, ICategoryRepository categoryRepository)
    {
        _pricesProvider = pricesProvider;
        _assetRepository = assetRepository;
        _categoryRepository = categoryRepository;
    }

    private async Task<List<TimelineDataItem>> GetHistoricalStockPriceAsync(string symbol)
    {
        var toDate = DateTime.Now;
        var fromDate = DateTime.Now.AddDays(-365);

        return (await _pricesProvider.GetHistoricalPriceAsync(symbol, fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"))).ToList();
    }

    public async Task<TimelineSummaryDto> GetHistoricalCategoryPriceAsync(string userId)
    {
        var categories = _categoryRepository.GetUserCategories(userId);
        var overallAggregatedPrices = new Dictionary<DateTime, double>();
        var categorySummaries = new List<TimelineSummaryDto>();

        foreach (var category in categories)
        {
            var assets = _assetRepository.GetAssetsByCategory(userId, category.Id);
            var categoryAggregatedPrices = new Dictionary<DateTime, double>();

            foreach (var asset in assets)
            {
                var assetPrices = await GetHistoricalStockPriceAsync(asset.Ticker);
                foreach (var priceItem in assetPrices)
                {
                    if (!categoryAggregatedPrices.ContainsKey(priceItem.Date))
                    {
                        categoryAggregatedPrices[priceItem.Date] = 0;
                    }
                    categoryAggregatedPrices[priceItem.Date] += priceItem.Price;

                    if (!overallAggregatedPrices.ContainsKey(priceItem.Date))
                    {
                        overallAggregatedPrices[priceItem.Date] = 0;
                    }
                    overallAggregatedPrices[priceItem.Date] += priceItem.Price;
                }
            }

            var categoryTimelineItems = categoryAggregatedPrices.Select(kvp => new TimelineDataItem
            {
                Date = kvp.Key,
                Price = kvp.Value
            }).ToList();

            categorySummaries.Add(new TimelineSummaryDto
            {
                Name = category.Name,
                Prices = categoryTimelineItems
            });
        }

        var overallTimelineItems = overallAggregatedPrices.Select(kvp => new TimelineDataItem
        {
            Date = kvp.Key,
            Price = kvp.Value
        }).ToList();

        return new TimelineSummaryDto
        {
            Name = "all",
            Prices = overallTimelineItems,
            Components = categorySummaries
        };
    }
}
