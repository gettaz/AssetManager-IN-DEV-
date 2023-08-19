using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface IAssetRepository
    {
        IEnumerable<Asset> GetUserAssets(string userId);
        IEnumerable<Asset> GetAssetsByBroker(string userId, string brokerName);
        IEnumerable<Asset> GetAssetsByCategory(string userId, int category);
        IEnumerable<Asset> GetPastHoldings(string userId);
        bool CreateAsset(Asset asset);
        bool UpdateAsset(Asset asset);
        bool DeleteAsset(string userId, int assetId);
        public bool AddAssetToCategory(string userId, int assetId, int categoryId);
        public bool RemoveAssetFromCategory(string userId, int assetId, int categoryId);

    }
}
