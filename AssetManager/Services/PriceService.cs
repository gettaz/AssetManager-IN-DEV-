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
        var cachedData = _priceRepository.GetHistoricalPrice(symbol, fromDate, toDate);
        IEnumerable<TimelineDataItem> timelineDataItems = Array.Empty<TimelineDataItem>();
        var toDateParsed = DateTime.Parse(toDate);
        var fromDateParsed = DateTime.Parse(fromDate);

        if (cachedData != null && cachedData.Any())
        {
            timelineDataItems = cachedData.Select(pr => new TimelineDataItem
            {
                Date = pr.Date,
                Price = pr.Value
            }).ToList();

            fromDateParsed = timelineDataItems.OrderByDescending(td => td.Date).FirstOrDefault().Date;
        }

        if (fromDateParsed != toDateParsed)
        {
            var fetchedData = (await _pricesProvider.GetHistoricalPriceAsync(symbol, fromDateParsed.ToString(), toDate)).Prices.Select(td => new Price
            {
                Date = td.Date,
                Value = td.Price,
                Ticker = symbol
            }).ToList();

            _priceRepository.AddPriceData(symbol, fetchedData);
        }


        return new TimelineSummaryDto
        {
            Name = symbol,
            Prices = timelineDataItems
        }; ;
    }
}
