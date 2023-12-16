using AssetManager.DTO;
using System.Threading.Tasks;

public interface IPriceService
{
    Task<TimelineSummaryDto> GetHistoricalPriceAsync(string symbol, string fromDate, string toDate);
}
