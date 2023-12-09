using AssetManager.DTO;

namespace AssetManager.Interfaces
{
    public interface IAssetClassificationsService
    {
        IEnumerable<ClassificationDto> GetCategories(string userId);
        IEnumerable<ClassificationDto> GetBrokers(string userId);
        bool CreateCategory(string userId, ClassificationDto categoryDto);
        bool UpdateCategory(string userId, ClassificationDto categoryDto);
        bool DeleteCategory(string userId, int categoryId);
        bool CreateBroker(string userId, ClassificationDto brokerDto);
        bool UpdateBroker(string userId, ClassificationDto brokerDto);
        bool DeleteBroker(string userId, int brokerId);
    }
}
