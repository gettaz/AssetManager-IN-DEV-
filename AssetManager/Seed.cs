using AssetManager.Data;
using AssetManager.Models;
using Microsoft.AspNetCore.Identity;

namespace AssetManager
{
    public class Seed
    {
        private readonly DataContext context;
        private readonly UserManager<IdentityUser> _userManager;

        public Seed(DataContext context, UserManager<IdentityUser> userManger)
        {
            _userManager = userManger;
            this.context = context;
        }

        public void SeedDataContext()
        {
            if (!context.Assets.Any())
            {
                var user1 = new IdentityUser { UserName = "User1" };
                var user2 = new IdentityUser { UserName = "User2" };
                var user3 = new IdentityUser { UserName = "User3" };
                _userManager.CreateAsync(user1, "Password123!").Wait();
                _userManager.CreateAsync(user2, "Password123!").Wait();
                _userManager.CreateAsync(user3, "Password123!").Wait();

                var assets = new List<Asset>
                {
                    new Asset
                    {
                        User = user1,
                        AssetName = "Apple Inc.",
                        Ticker = "AAPL",
                        PriceBought = 150.0,
                        BrokerName = "Broker1",
                        AssetCategories = new List<AssetCategory>
                            { new AssetCategory() { Category = new Category() { Name = "Technology" } } },
                        DateBought = new DateTime(2021, 1, 1),
                        DateSold = new DateTime(2022, 1, 1)
                    },
                    new Asset
                    {
                        User = user1,
                        AssetName = "Microsoft Corp.",
                        Ticker = "MSFT",
                        PriceBought = 200.0,
                        BrokerName = "Broker1",
                        AssetCategories = new List<AssetCategory>()
                        {
                            new AssetCategory() { Category = new Category() { Name = "Computers" } },
                            new AssetCategory() { Category = new Category() { Name = "Technology" } }
                        },
                        DateBought = new DateTime(2021, 2, 1),
                        DateSold = null
                    },
                    new Asset
                    {
                        User = user2,
                        AssetName = "Amazon Inc.",
                        Ticker = "AMZN",
                        PriceBought = 2500.0,
                        BrokerName = "Broker2",
                        AssetCategories = new List<AssetCategory>
                            { new AssetCategory() { Category = new Category() { Name = "Ecommerce" } } },
                        DateBought = new DateTime(2022, 1, 1),
                        DateSold = new DateTime(2023, 1, 1)
                    },
                    new Asset
                    {
                        User = user3,
                        AssetName = "Tesla Inc.",
                        Ticker = "TSLA",
                        PriceBought = 650.0,
                        BrokerName = "Broker3",
                        AssetCategories = new List<AssetCategory>
                            { new AssetCategory() { Category = new Category() { Name = "Cars" } } },
                        DateBought = new DateTime(2022, 6, 1),
                        DateSold = null
                    },
                    new Asset
                    {
                        User = user2,
                        AssetName = "Walmart Inc.",
                        Ticker = "WMT",
                        PriceBought = 140.0,
                        BrokerName = "Broker2",
                        AssetCategories = new List<AssetCategory>()
                        {
                            new AssetCategory
                            {
                                Category = new Category() { Name = "Retail" }
                            }
                        },
                        DateBought = new DateTime(2022, 7, 1),
                        DateSold = new DateTime(2023, 7, 1)
                    }
                    // Add more Asset objects here as needed.
                };
                context.Assets.AddRange(assets);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}