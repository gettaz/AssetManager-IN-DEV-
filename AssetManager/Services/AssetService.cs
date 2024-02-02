using AssetManager.DTO;
using AssetManager.Interfaces;
using AssetManager.Models;
using AssetManager.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace AssetManager.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBrokerRepository _brokerRepository;

        private readonly IMapper _mapper;

        public AssetService(IAssetRepository assetRepository, ICategoryRepository categoryRepository, IBrokerRepository brokerRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _categoryRepository = categoryRepository;
            _brokerRepository = brokerRepository;
            _mapper = mapper;
        }
        public IEnumerable<AssetDto> GetUserAssets(string userId)
        {
            var assets = _assetRepository.GetUserAssets(userId).Where(asset => asset.DateSold == null).AsEnumerable();
            var result = new List<AssetDto>();
            
            foreach (var asset in assets) 
            {
                result.Add(_mapper.Map<AssetDto>(asset));
            }

            return result;
        }
        public IEnumerable<ConsolidatedAssetDto> GetUserConsolidatedAssets(string userId)
        {
            var assets = _assetRepository.GetUserAssets(userId).Where(asset => asset.DateSold == null).AsEnumerable();

            var consolidatedAssets = assets
                .GroupBy(a => new { a.AssetName, BrokerName = a.Broker?.Name, CategoryName = a.Category?.Name })
                .Select(group => new ConsolidatedAssetDto
                {
                    AssetName = group.Key.AssetName,
                    Ticker = group.First().Ticker,
                    TotalAmount = group.Sum(a => a.Amount),
                    AveragePriceBought = GetAveragePriceBought(group),
                    BrokerName = group.Key?.BrokerName, 
                    Category = group.Key?.CategoryName
                })
                .OrderBy(ca => ca.AssetName)
                .ToList();

            return consolidatedAssets;
        }

        public bool CreateAsset(AssetDto assetDto, string userId)
        {
            if (assetDto == null)
            {
                return false; 
            }
            var asset = _mapper.Map<Asset>(assetDto);

            asset.UserId = userId;
            asset.Category = GetClassification(assetDto.CategoryName,
                _categoryRepository.GetUserCategories(userId));

            asset.Broker = GetClassification(assetDto.BrokerName,
                _brokerRepository.GetUserBrokers(userId));

            if (asset.Category == null && assetDto.CategoryName != null)
            {
                var category = new Category() { Name = assetDto.CategoryName, UserId = userId };
                _categoryRepository.CreateCategory(category);
                asset.Category = category;
            }

            if (asset.Broker == null && assetDto.BrokerName != null)
            {
                var broker = new Broker() { Name = assetDto.BrokerName, UserId = userId };
                _brokerRepository.CreateBroker(broker);
                asset.Broker = broker;
            }

            return _assetRepository.CreateAsset(asset);
        }

        private T? GetClassification<T>(string name, IEnumerable<T> classifications) where T : Classification
        {
            return classifications
                       .FirstOrDefault(cla => cla.Name == name);
        }

        private double GetAveragePriceBought(IEnumerable<Asset> assets)
        {
            double totalAmount = 0;
            double totalPrice = 0;
            foreach(var asset in assets)
            {
                totalAmount += asset.Amount;
                totalPrice += asset.Amount * asset.PriceBought;
            }

            return totalPrice / totalAmount;
        }
    }
}
