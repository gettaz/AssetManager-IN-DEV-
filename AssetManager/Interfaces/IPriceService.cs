using AssetManager.DTO;

public interface IPriceService
{
    Task<TimelineSummaryDto> GetHistoricalCategoryPriceAsync(string userId);

    Task<TimelineSummaryDto> GetHistoricalBrokerPriceAsync(string userId);

    Task<TimelineSummaryDto> GetHistoricalAllPriceAsync(string userId);


    Task<IEnumerable<string>> GetAllowedTickers();
}
