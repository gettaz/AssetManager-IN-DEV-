using AssetManager.DTO;
using AssetManager.Models;

namespace AssetManager.Repository
{
    public interface IPriceRepository
    {
        IEnumerable<Price> GetHistoricalPrice(string symbol, string fromDate, string toDate);
        bool AddPriceData(string symbol, IEnumerable<Price> priceData);
    }
}
