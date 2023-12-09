using AssetManager.DTO;
using AssetManager.Interfaces;
using AssetManager.Models;
using AutoMapper;

namespace AssetManager.Services
{
    public class AssetClassificationsService : IAssetClassificationsService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrokerRepository _brokerRepository;
        private readonly IMapper _mapper;

        public AssetClassificationsService(ICategoryRepository categoryRepository,
                                           IBrokerRepository brokerRepository,
                                           IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _brokerRepository = brokerRepository;
            _mapper = mapper;
        }

        public IEnumerable<ClassificationDto> GetCategories(string userId)
        {
            var categories = _categoryRepository.GetUserCategories(userId);
            return _mapper.Map<IEnumerable<ClassificationDto>>(categories);
        }

        public IEnumerable<ClassificationDto> GetBrokers(string userId)
        {
            var brokers = _brokerRepository.GetUserBrokers(userId);
            return _mapper.Map<IEnumerable<ClassificationDto>>(brokers);
        }

        public bool CreateCategory(string userId, ClassificationDto categoryDto)
        {
            if (categoryDto == null)
            {
                return false;
            }

            var existingCategories = _categoryRepository.GetUserCategories(userId);
            if (existingCategories.Any(c => c.Name.Equals(categoryDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                // Category with the same name already exists
                return false;
            }

            var category = _mapper.Map<Category>(categoryDto);
            category.UserId = userId; // Assign the UserId to the new category

            return _categoryRepository.CreateCategory(category);
        }

        public bool UpdateCategory(string userId, ClassificationDto categoryDto)
        {
            if (categoryDto == null)
            {
                return false;
            }

            var existingCategories = _categoryRepository.GetUserCategories(userId);
            if (existingCategories.Any(c => c.Name.Equals(categoryDto.Name, StringComparison.OrdinalIgnoreCase) && c.Id != categoryDto.Id))
            {
                // Another category with the same name already exists
                return false;
            }

            var categoryToUpdate = existingCategories.FirstOrDefault(c => c.Id == categoryDto.Id);
            if (categoryToUpdate == null)
            {
                // No category found with the given ID
                return false;
            }

            categoryToUpdate.Name = categoryDto.Name;
            return _categoryRepository.UpdateCategory(categoryToUpdate);
        }

        public bool DeleteCategory(string userId, int categoryId)
        {
            return _categoryRepository.DeleteCategory(userId, categoryId);
        }

        public bool CreateBroker(string userId, ClassificationDto brokerDto)
        {
            if (brokerDto == null)
            {
                return false;
            }

            var existingBrokers = _brokerRepository.GetUserBrokers(userId);
            if (existingBrokers.Any(b => b.Name.Equals(brokerDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                // Broker with the same name already exists
                return false;
            }

            var broker = _mapper.Map<Broker>(brokerDto);
            broker.UserId = userId; // Assign the UserId to the new broker

            return _brokerRepository.CreateBroker(broker);
        }

        public bool UpdateBroker(string userId, ClassificationDto brokerDto)
        {
            if (brokerDto == null)
            {
                return false;
            }

            var existingBrokers = _brokerRepository.GetUserBrokers(userId);
            if (existingBrokers.Any(b => b.Name.Equals(brokerDto.Name, StringComparison.OrdinalIgnoreCase) && b.Id != brokerDto.Id))
            {
                // Another broker with the same name already exists
                return false;
            }

            var brokerToUpdate = existingBrokers.FirstOrDefault(b => b.Id == brokerDto.Id);
            if (brokerToUpdate == null)
            {
                // No broker found with the given ID
                return false;
            }

            brokerToUpdate.Name = brokerDto.Name;
            return _brokerRepository.UpdateBroker(brokerToUpdate);
        }

        public bool DeleteBroker(string userId, int brokerId)
        {
            return _brokerRepository.DeleteBroker(userId, brokerId);
        }
    }
}
