using AssetManager.Models;

namespace AssetManager.Interfaces
{//:TODO controler should have same calls too
    public interface IFinancialDataService
    {
        //Both sould use sockets
        Task<IEnumerable<AssetSummary>> GetPossibleAssets(string input);
        Task<IEnumerable<AssetSummary>> GetCurrentPrices(IEnumerable<Asset> assetTicker);

    }
}
