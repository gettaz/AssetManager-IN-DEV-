using AssetManager.DTO;
using AssetManager.Interfaces;
using AssetManager.Models;

public class PriceService : IPriceService
{
    private readonly IHistoricalPriceProvider _pricesProvider;
    private readonly ICurrentPriceProvider _currPricesProvider;
    private readonly IAssetRepository _assetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrokerRepository _brokerRepository;


    public PriceService(IHistoricalPriceProvider pricesProvider, ICurrentPriceProvider currPricesProvider, IAssetRepository assetRepository, ICategoryRepository categoryRepository, IBrokerRepository brokerRepository)
    {
        _pricesProvider = pricesProvider;
        _currPricesProvider = currPricesProvider;
        _assetRepository = assetRepository;
        _categoryRepository = categoryRepository;
        _brokerRepository = brokerRepository;
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
                    if (priceItem != null && !categoryAggregatedPrices.ContainsKey(priceItem.Date))
                    {
                        categoryAggregatedPrices[priceItem.Date] = 0;
                    }
                    categoryAggregatedPrices[priceItem!.Date] += priceItem.Price;

                    if (priceItem != null && !overallAggregatedPrices.ContainsKey(priceItem.Date))
                    {
                        overallAggregatedPrices[priceItem.Date] = 0;
                    }
                    overallAggregatedPrices[priceItem!.Date] += priceItem.Price;
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
            Name = "category",
            Prices = overallTimelineItems,
            Components = categorySummaries
        };
    }

    public async Task<TimelineSummaryDto> GetHistoricalBrokerPriceAsync(string userId)
    {
        var brokers = _brokerRepository.GetUserBrokers(userId);
        var overallAggregatedPrices = new Dictionary<DateTime, double>();
        var brokerSummaries = new List<TimelineSummaryDto>();

        foreach (var broker in brokers)
        {
            var assets = _assetRepository.GetAssetsByBroker(userId, broker.Id);
            var brokerAggregatedPrices = new Dictionary<DateTime, double>();

            foreach (var asset in assets)
            {
                var assetPrices = await GetHistoricalStockPriceAsync(asset.Ticker);
                foreach (var priceItem in assetPrices)
                {
                    if (priceItem != null && !brokerAggregatedPrices.ContainsKey(priceItem.Date))
                    {
                        brokerAggregatedPrices[priceItem.Date] = 0;
                    }
                    brokerAggregatedPrices[priceItem!.Date] += priceItem.Price;

                    if (priceItem != null && !overallAggregatedPrices.ContainsKey(priceItem.Date))
                    {
                        overallAggregatedPrices[priceItem.Date] = 0;
                    }
                    overallAggregatedPrices[priceItem!.Date] += priceItem.Price;
                }
            }

            var categoryTimelineItems = brokerAggregatedPrices.Select(kvp => new TimelineDataItem
            {
                Date = kvp.Key,
                Price = kvp.Value
            }).ToList();

            brokerSummaries.Add(new TimelineSummaryDto
            {
                Name = broker.Name,
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
            Name = "broker",
            Prices = overallTimelineItems,
            Components = brokerSummaries
        };
    }

    public async Task<TimelineSummaryDto> GetHistoricalAllPriceAsync(string userId)
    {
        var overallAggregatedPrices = new Dictionary<DateTime, double>();
        var assetSummaries = new List<TimelineSummaryDto>();
        var assets = _assetRepository.GetUserAssets(userId);

        foreach (var asset in assets)
        {
            var assetAggregatedPrices = new Dictionary<DateTime, double>();
            var assetPrices = await GetHistoricalStockPriceAsync(asset.Ticker);

            foreach (var priceItem in assetPrices)
            {
                if (priceItem != null && !assetAggregatedPrices.ContainsKey(priceItem.Date))
                {
                    assetAggregatedPrices[priceItem.Date] = 0;
                }
                assetAggregatedPrices[priceItem!.Date] += priceItem.Price;

                if (priceItem != null && !overallAggregatedPrices.ContainsKey(priceItem.Date))
                {
                    overallAggregatedPrices[priceItem.Date] = 0;
                }
                overallAggregatedPrices[priceItem!.Date] += priceItem.Price;
            }


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
            Components = assetSummaries
        };
    }

    public async Task<IEnumerable<string>> GetAllowedTickers()
    {
        var currAllowedTask = _currPricesProvider.GetTickers();
        var historicalTask = _pricesProvider.GetTickers();
        await Task.WhenAll(currAllowedTask, historicalTask);

        var currAllowed = await currAllowedTask;
        var historical = await historicalTask;

        return currAllowed.Intersect(historical);
    }
}
