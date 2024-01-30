using AssetManager.DTO;

public interface IPriceService
{
    Task<TimelineSummaryDto> GetHistoricalCategoryPriceAsync(string userId);
}
