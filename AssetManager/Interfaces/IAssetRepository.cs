﻿using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface IAssetRepository
    {
        IEnumerable<Asset> GetUserAssets(string userId);
        IEnumerable<Asset> GetAssetsByBroker(string userId, string brokerName);
        IEnumerable<Asset> GetAssetsByCategory(string userId, string category);
        IEnumerable<Asset> GetPastHoldings(string userId);
        bool CreateAsset(Asset asset);
        bool UpdateAsset(Asset asset);
        bool DeleteAsset(int assetId);
        public bool AddAssetToCategory(string userId, int assetId, string categoryName);
    }
}
