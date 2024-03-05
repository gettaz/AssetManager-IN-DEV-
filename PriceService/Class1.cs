using Xunit;
using Moq;
using AssetManager.Interfaces;
using AutoMapper;
using AssetManager.DTO;
using AssetManager.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Prime.UnitTests.Services
{
    public class PriceService_GetHistoricalAllPrices
    {
        PriceService priceService;
        private readonly Mock<IAssetRepository> _mockAssetRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IBrokerRepository> _mockBrokerRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICurrentPriceProvider> _mockCurrentPriceProvider;
        private readonly Mock<IHistoricalPriceProvider> _mockHistoricPricesProvider;

        private static IDictionary<DateTime, double> _yearlyPrices;
    
        public PriceService_GetHistoricalAllPrices()
        {
            _yearlyPrices = GenerateRandomDailyValuesForYear();
            _mockHistoricPricesProvider = new Mock<IHistoricalPriceProvider>(MockBehavior.Strict);
            _mockCurrentPriceProvider = new Mock<ICurrentPriceProvider>(MockBehavior.Strict);
            _mockAssetRepository = new Mock<IAssetRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockBrokerRepository = new Mock<IBrokerRepository>();
            _mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public async void PriceService_GetAsspplHistoricalMonthPrices()
        {
            var assets = GenerateUserAssets();
            var amounts = GenerateYearlyAmounts(assets);

            _mockHistoricPricesProvider.Setup(provider => provider.GetHistoricalPriceAsync(It.IsAny<string>(), It.IsAny<string>(),
                                It.IsAny<string>())).ReturnsAsync(_yearlyPrices.Select( (k) => new TimelineDataItem() { Date = k.Key, Price = k.Value}));

            priceService = new(_mockHistoricPricesProvider.Object, _mockCurrentPriceProvider.Object,
                _mockAssetRepository.Object, _mockCategoryRepository.Object, _mockBrokerRepository.Object);
            _mockAssetRepository.Setup(rassets => rassets.GetUserAssets(It.IsAny<string>())).Returns(assets);
            var expected = new TimelineSummaryDto();
            expected.Name = "all";
            expected.Prices = GenerateYearlyExpectedPrices(_yearlyPrices, amounts, CalculateAverageStockPricePerDay(assets, amounts));

            var result = await priceService.GetHistoricalAllPriceAsync("hi");

            Assert.True(expected.Prices.SequenceEqual(result.Prices));
            Assert.Same(expected.Name, result.Name);
        }

        public static List<TimelineDataItem> GenerateYearlyExpectedPrices(IDictionary<DateTime, double> stockPrices, IDictionary<DateTime, double> stockAmounts, IDictionary<DateTime, double> stockAvgPriceBoughtAmounts)
        {
            List<TimelineDataItem> timelineDataItems = new List<TimelineDataItem>();

            for (int day = 365; day >= 0; day--)
            {
                var date = DateTime.Now.AddDays(-day).Date;
                timelineDataItems.Add(new TimelineDataItem
                {
                    Date = date,
                    Price = stockPrices[date] * stockAmounts[date] - stockAmounts[date] * stockAvgPriceBoughtAmounts[date]
                }); ;
            }

            return timelineDataItems;
        }

        public static IDictionary<DateTime, double> CalculateAverageStockPricePerDay(IEnumerable<Asset> userAssets, IDictionary<DateTime, double> amountPerDay)
        {
            IDictionary<DateTime, double> averagePrices = new Dictionary<DateTime, double>();

            IDictionary<DateTime, double> dailyTotalPrice = new Dictionary<DateTime, double>();

            foreach (var asset in userAssets)
            {
                foreach (var date in amountPerDay.Keys)
                {
                    if (asset.DateBought <= date)
                    {
                        if (!dailyTotalPrice.ContainsKey(date))
                        {
                            dailyTotalPrice[date] = 0;
                        }
                        dailyTotalPrice[date] += asset.PriceBought * asset.Amount;
                    }
                }
            }

            foreach (var date in amountPerDay.Keys)
            {
                if (amountPerDay[date] > 0 && dailyTotalPrice.ContainsKey(date)) 
                {
                    averagePrices[date] = dailyTotalPrice[date] / amountPerDay[date];
                }
                else
                {
                    averagePrices[date] = 0; 
                }
            }

            return averagePrices;
        }


        public static IDictionary<DateTime, double> GenerateYearlyAmounts(IEnumerable<Asset> userAssets)
        {
            IDictionary<DateTime, double> yearlyAmounts = new Dictionary<DateTime, double>();

            for (int day = 365; day >= 0; day--)
            {
                var date = DateTime.Now.AddDays(-day).Date;
                yearlyAmounts[date] = 0;

                foreach (var asset in userAssets)
                {
                    if (asset.DateBought < date)
                    {
                        yearlyAmounts[date] += asset.Amount;                        
                    }
                }

            }

            return yearlyAmounts;
        }

        public static IEnumerable<Asset> GenerateUserAssets()
        {
            var assetDto = new Asset
            {
                AssetName = "AAPL",
                Id = 0,
                Ticker = "AAPL",
                PriceBought = 2,
                Amount = 1,
                DateBought = DateTime.Now.AddDays(-366).Date
            };

            return new List<Asset>() { assetDto };
        }

        public static IDictionary<DateTime, double> GenerateRandomDailyValuesForYear()
        {
            Random rnd = new Random();

            IDictionary<DateTime, double> timelineDataItems = new Dictionary<DateTime, double>();

            for (int day = 0; day <= 365; day++)
            {
                double num = rnd.Next();

                timelineDataItems.Add(
                    DateTime.Now.AddDays(-day).Date,
                    num 
                );
            }

            return timelineDataItems;
        }

    }
}