using AssetManager.DTO;

public interface IPriceService
{
    Task<TimelineSummaryDto> GetHistoricalCategoryPriceAsync(string userId);

    Task<IEnumerable<string>> GetAllowedTickers();
}
