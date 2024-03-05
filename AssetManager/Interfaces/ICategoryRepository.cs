using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetUserCategories(string userId);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(string userId, int categoryId);
        int GetCategoryId(string userId, string? name);
        bool Save();
    }
}
