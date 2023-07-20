using AssetManager.Data;
using AssetManager.Interfaces;
using AssetManager.Models;

namespace AssetManager.Repository
{
    public class AssetRepository : IAssetRepository
    {
        private DataContext _context;
        public AssetRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Asset> GetUserAssets(string userId)
        {
            return _context.Assets.Where(a => a.UserId == userId).ToList();
        }


        public bool CreateAsset(Asset asset)
        {
             _context.Add(asset);
             return Save();
        }

        public bool UpdateAsset(Asset asset)
        {
            _context.Attach(asset);

            var entry = _context.Entry(asset);

            // For each property in the asset, mark it as modified if it is not null
            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue != null && property.Metadata.IsForeignKey())
                {
                    property.IsModified = true;
                }
            }

            return Save();
        }


        public bool DeleteAsset(int assetId)
        {
            var asset = _context.Assets.FirstOrDefault(a => a.Id == assetId);
            if (asset == null)
            {
                return false; 
            }
            foreach (var ac in asset.AssetCategories)
            {
                _context.AssetsCategories.Remove(ac);
            }
            _context.Assets.Remove(asset);
            return Save(); 
        }

        public ICollection<Asset> GetPastHoldings(string userId)
        {
            var userAssets = GetUserAssets(userId);
            return userAssets.Where(a => a.DateSold != null).ToList();
        }

        public ICollection<Asset> GetAssetsByBroker(string userId, string brokerName)
        {
            var userAssets = GetUserAssets(userId);
            return userAssets.Where(a => a.BrokerName == brokerName).ToList();
        }

        public ICollection<Asset> GetAssetsByCategory(string userId, int categoryId)
        {
            var userAssets = GetUserAssets(userId);
            return userAssets.Where(a => a.AssetCategories.
                Any(ac => ac.CategoryId == categoryId)).ToList();
        }
        private bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
