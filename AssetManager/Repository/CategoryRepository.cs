using AssetManager.Data;
using AssetManager.Interfaces;
using AssetManager.Models;

namespace AssetManager.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private DataContext _context;
        public CategoryRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateCategory(Category category)
        {
            _context.Add(category);
            return Save();
        }

        public bool DeleteCategory(int categoryId)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
            {
                return false;
            }

            var rem = _context.AssetsCategories.Where(ac => ac.CategoryId == categoryId);

            foreach (var ac in rem)
            { 
                _context.AssetsCategories.Remove(ac);        
            }

            _context.Categories.Remove(category);

            return Save();
        }

        public IEnumerable<Category> GetUserCategories(string userId)
        {
            return _context.Categories.Where(c => c.UserId == userId).ToList();
        }

        public IEnumerable<Category> GetAssetCategories(int assetId)
        {
            return _context.AssetsCategories.Where(ac => ac.AssetId == assetId)
                .Select(ac => ac.Category).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _context.Update(category);
            return Save();
        }
    }
}
