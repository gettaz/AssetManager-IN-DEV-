using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface ICategoryRepository
    {
        //TODO: search by name in sql- instead of get all
        IEnumerable<Category> GetUserCategories(string userId);
        bool CreateCategory(Category category);
        bool UpdateCategory(Category category);
        bool DeleteCategory(string userId, int categoryId);
        bool Save();
    }
}
