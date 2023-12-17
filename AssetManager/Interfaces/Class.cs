using AssetManager.DTO;
using System;

namespace AssetManager.Interfaces
{
    public interface  IPriceProvider
    {
        Task<TimelineSummaryDto> GetHistoricalPriceAsync(string symbol, string fromDate, string toDate);
    }
}
