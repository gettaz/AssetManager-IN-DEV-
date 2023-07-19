using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetUserCategories(string userId);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(int categoryId);
        bool Save();
    }
}
