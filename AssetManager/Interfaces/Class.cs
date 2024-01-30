using AssetManager.DTO;
using System;

namespace AssetManager.Interfaces
{
    public interface  IPriceProvider
    {
        Task<IEnumerable<TimelineDataItem>> GetHistoricalPriceAsync(string symbol, string fromDate, string toDate);
    }
}
