using AssetManager.Data;
using AssetManager.Interfaces;
using AssetManager.Models;
using Microsoft.EntityFrameworkCore;

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
            return _context.Assets
                           .Where(a => a.UserId == userId)
                           .Include(a => a.Broker) // Eagerly load the Broker
                           .Include(a => a.Category) // Eagerly load the Category
                           .ToList();
        }

        public bool CreateAsset(Asset asset)
        {
            // Add and save the asset
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


        public bool DeleteAsset(string userId, int assetId)
        {
            var asset = GetUserAssets(userId).FirstOrDefault(a => a.Id == assetId);

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

        public IEnumerable<Asset> GetAssetsByBroker(string userId, int brokerId)
        {
            var assetByBroker = _context.Assets.Where(ac => ac.BrokerId == brokerId && ac.UserId == userId).Select(ac => ac).ToList();
            if (assetByBroker.Any())
            {
                return assetByBroker;
            }
            return Enumerable.Empty<Asset>();
        }

        public IEnumerable<Asset> GetAssetsByCategory(string userId, int id)
        {
            var assetByCategory = _context.Assets.Where(ac => ac.Category.Id == id && ac.Category.UserId == userId).Select(ac => ac).ToList();

            if (assetByCategory.Any())
            {
                return assetByCategory;
            }
            return Enumerable.Empty<Asset>();
        }
        private bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(string userId, int assetId, int categoryId)
        {
            var asset = _context.Assets.FirstOrDefault(a => a.UserId == userId && a.Id == assetId);
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (asset == null || category == null)
            {
                return false;
            }

            asset.Category = category;
            return Save();
        }

        public bool RemoveAssetCategory(string userId, int assetId)
        {
            var asset = _context.Assets.FirstOrDefault(a => a.UserId == userId && a.Id == assetId);

            if (asset == null)
            {
                return false;
            }

            asset.Category = null;
            return Save();
        }
    }
}
