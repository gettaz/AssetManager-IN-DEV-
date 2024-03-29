﻿using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface IAssetRepository
    {
        IEnumerable<Asset> GetUserAssets(string userId);
        IEnumerable<Asset> GetAssetsByBroker(string userId, int broker);
        IEnumerable<Asset> GetAssetsByCategory(string userId, int category);
        IEnumerable<Asset> GetAssetsDetails(string userId, int broker, int category, string ticker);

        IEnumerable<Asset> GetPastHoldings(string userId);
        bool CreateAsset(Asset asset);
        bool UpdateAsset(Asset asset);
        bool DeleteAsset(string userId, int assetId);
        public bool UpdateCategory(string userId, int assetId, int categoryId);
        public bool RemoveAssetCategory(string userId, int assetId);

    }
}
