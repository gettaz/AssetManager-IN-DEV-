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

        public IEnumerable<Asset> GetUserAssets(string userId)
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
            
            _context.Remove(asset);
            return Save(); 
        }

        public IEnumerable<Asset> GetPastHoldings(string userId)
        {
            var userAssets = GetUserAssets(userId);
            return userAssets.Where(a => a.DateSold != null).ToList();
        }

        public IEnumerable<Asset> GetAssetsByBroker(string userId, string brokerName)
        {
            var userAssets = GetUserAssets(userId);
            return userAssets.Where(a => a.BrokerName == brokerName).ToList();
        }

        public IEnumerable<Asset> GetAssetsByCategory(string userId, int id)
        {
            var assetByCategory = _context.AssetsCategories.Where(ac => ac.Category.Id == id && ac.Category.UserId == userId).Select(ac => ac.Asset).ToList();

            if (assetByCategory.Any())
            {
                return assetByCategory;
            }
            return null;
        }
        private bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool AddAssetToCategory(string userId, int assetId, int categoryId)
        {
            var asset = _context.Assets.FirstOrDefault(a => a.UserId == userId && a.Id == assetId);
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (asset == null || category == null)
            {
                return false;
            }

            var assetCategory = new AssetCategory
            {
                AssetId = assetId,
                CategoryId = category.Id
            };

            _context.AssetsCategories.Add(assetCategory);
            return Save();
        }

        public bool RemoveAssetFromCategory(string userId, int assetId, int categoryId)
        {
            var asset = _context.Assets.FirstOrDefault(a => a.UserId == userId && a.Id == assetId);
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (asset == null || category == null)
            {
                return false;
            }

            var assetCategory = new AssetCategory
            {
                AssetId = assetId,
                CategoryId = category.Id
            };

            _context.AssetsCategories.Remove(assetCategory);
            return Save();
        }
    }
}
