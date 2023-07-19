using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface IAssetRepository
    {
        ICollection<Asset> GetUserAssets(string userId);
        ICollection<Asset> GetAssetsByBroker(string userId, string brokerName);
        ICollection<Asset> GetAssetsByCategory(string userId, int categoryId);
        ICollection<Asset> GetPastHoldings(string userId);
        bool CreateAsset(Asset asset);
        bool UpdateAsset(Asset asset);
        bool DeleteAsset(int assetId);
 }
}
