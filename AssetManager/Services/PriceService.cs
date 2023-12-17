using AssetManager.DTO;
using AssetManager.Interfaces;
using AssetManager.Models;
using AssetManager.Repository;

public class PriceService : IPriceService
{
    private readonly IPriceProvider _pricesProvider;
    private readonly IPriceRepository _priceRepository;

    public PriceService(IPriceProvider pricesProvider, IPriceRepository priceRepository)
    {
        _pricesProvider = pricesProvider;
        _priceRepository = priceRepository;
    }

    public async Task<TimelineSummaryDto> GetHistoricalPriceAsync(string symbol, string fromDate, string toDate)
    {
        var toDateParsed = DateTime.Parse(toDate);
        var fromDateParsed = DateTime.Parse(fromDate);
        var cachedData = _priceRepository.GetHistoricalPrice(symbol, fromDateParsed, toDateParsed);
        IEnumerable<Price> dbPrices = Array.Empty<Price>();
        IEnumerable<Price> fetchedData = Array.Empty<Price>();

        var dbToDate = DateTime.Now.AddDays(-365);
        var dbFromDate = DateTime.Now;

        if (cachedData != null && cachedData.Any())
        {
            dbPrices = cachedData.OrderByDescending(td => td.Date);

            dbToDate = dbPrices.FirstOrDefault().Date;
            dbFromDate = dbPrices.LastOrDefault().Date;
        }
        else
        {
           //TODO: get from a year back in initial insertion 
            dbPrices = (await _pricesProvider.GetHistoricalPriceAsync(symbol, fromDate, toDate)).Prices.Select(td => new Price
            {
                Date = td.Date,
                Value = td.Price,
                Ticker = symbol
            }).ToList();
            _priceRepository.AddPriceData(symbol, dbPrices);
            dbFromDate = DateTime.Parse(fromDate);
            dbToDate = DateTime.Parse(toDate);

            return new TimelineSummaryDto
            {
                Name = symbol,
                Prices = dbPrices.Select(pr => new TimelineDataItem
                {
                    Date = pr.Date,
                    Price = pr.Value
                }).ToList()
            };
        }

        if (!(dbFromDate <= fromDateParsed))
        {
            fetchedData = (await _pricesProvider.GetHistoricalPriceAsync(symbol, fromDateParsed.ToString("yyyy-MM-dd"), dbFromDate.AddDays(-1).ToString("yyyy-MM-dd"))).Prices.Select(td => new Price
            {
                Date = td.Date,
                Value = td.Price,
                Ticker = symbol
            }).ToList();
            _priceRepository.AddPriceData(symbol, fetchedData);
        }

        if (!(dbToDate >= toDateParsed))
        {
            fetchedData = (await _pricesProvider.GetHistoricalPriceAsync(symbol, dbToDate.AddDays(1).ToString("yyyy-MM-dd"), toDateParsed.ToString("yyyy-MM-dd"))).Prices.Select(td => new Price
            {
                Date = td.Date,
                Value = td.Price,
                Ticker = symbol
            }).ToList();
            _priceRepository.AddPriceData(symbol, fetchedData);
        }

        var allPrices = dbPrices.Concat(fetchedData);

        return new TimelineSummaryDto
        {
            Name = symbol,
            Prices = allPrices.Select(pr => new TimelineDataItem
            {
                Date = pr.Date,
                Price = pr.Value
            }).ToList()
        };
    }
}
