using AssetManager.DTO;
using AssetManager.Interfaces;
using AssetManager.Models;
using System.Collections.Concurrent;
using System.Web.WebPages;

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
                    if (asset.DateBought < priceItem.Date)
                    {
                        if (!categoryAggregatedPrices.ContainsKey(priceItem.Date))
                        {
                            categoryAggregatedPrices[priceItem.Date] = 0;
                        }
                        categoryAggregatedPrices[priceItem!.Date] += (asset.PriceBought - priceItem.Price) * asset.Amount;
                    }
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

        return new TimelineSummaryDto
        {
            Name = "category",
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

        return new TimelineSummaryDto
        {
            Name = "broker",
            Components = brokerSummaries
        };
    }

    public async Task<TimelineSummaryDto> GetHistoricalAllPriceAsync(string userId)
    {
        var overallAggregatedPrices = new ConcurrentDictionary<DateTime, double>();

        for (var date = DateTime.Now.AddDays(-365).Date; date <= DateTime.Now.Date; date = date.AddDays(1))
        {
            overallAggregatedPrices.TryAdd(date.Date, 0);
        }

        var assets = _assetRepository.GetUserAssets(userId);

        var groupedAssets = assets.OrderBy(t => t.DateBought).GroupBy(ass => ass.Ticker);

        var assetsPricesPerDate = new Dictionary<string, 
                                        Dictionary<DateTime,
                                            Tuple<double, double>>>();

        foreach (var group in groupedAssets)
        {
            var yearAvgPrice = new Dictionary<DateTime, Tuple<double, double>>();

            for (int i = 0; i <= 365; ++i)
            {
                var rel = group.Where(t => t.DateBought <= DateTime.Now.AddDays(-i));
                var amountThatDay = rel.Select(r => r.Amount).Sum();
                var avgPriceThatDay = amountThatDay == 0 ? 0 : rel.Select(r => r.Amount * r.PriceBought).Sum() / amountThatDay;

                yearAvgPrice.Add(DateTime.Now.AddDays(-i).Date, new Tuple<double, double>(amountThatDay, avgPriceThatDay));
            }

            assetsPricesPerDate.Add(group.Key, yearAvgPrice);
        }

        await GetAssetsPricesAsync(overallAggregatedPrices, assetsPricesPerDate);

        var overallTimelineItems = overallAggregatedPrices.OrderBy(x => x.Key).Select(kvp => new TimelineDataItem
        {
            Date = kvp.Key,
            Price = kvp.Value
        }).ToList();

        return new TimelineSummaryDto
        {
            Name = "all",
            Prices = overallTimelineItems
        };
    }

    private async Task GetAssetsPricesAsync(ConcurrentDictionary<DateTime, double> overallAggregatedPrices, Dictionary<string,                 Dictionary<DateTime,
                                            Tuple<double, double>>> assets)
    {
        await Parallel.ForEachAsync(assets, async (asset, token) =>
        {
            var assetPrices = await GetHistoricalStockPriceAsync(asset.Key);

            foreach (var priceItem in assetPrices)
            {
                overallAggregatedPrices[priceItem.Date.Date] += (priceItem.Price - 
                    asset.Value[priceItem.Date.Date].Item2) * asset.Value[priceItem.Date.Date].Item1;
            }
        });
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
