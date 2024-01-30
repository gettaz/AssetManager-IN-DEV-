using AssetManager.Data;
using AssetManager.Models;
using Microsoft.AspNetCore.Identity;

namespace AssetManager
{
    public class Seed
    {
        private readonly DataContext context;
        private readonly UserManager<IdentityUser> _userManager;

        public Seed(DataContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
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

                var technologyCategory = new Category { Name = "Technology", UserId = user1.Id };
                var computersCategory = new Category { Name = "Computers", UserId = user1.Id };
                var ecommerceCategory = new Category { Name = "Ecommerce", UserId = user2.Id };
                var carsCategory = new Category { Name = "Cars", UserId = user3.Id };
                var retailCategory = new Category { Name = "Retail", UserId = user2.Id };

                var broker1 = new Broker { Name = "Broker1", UserId = user1.Id };
                var broker2 = new Broker { Name = "Broker2", UserId = user2.Id };
                var broker3 = new Broker { Name = "Broker3", UserId = user3.Id };

                var assets = new List<Asset>
                {
                    new Asset
                    {
                        UserId = user1.Id,
                        User = user1,
                        AssetName = "Apple Inc.",
                        Ticker = "AAPL",
                        PriceBought = 150.0,
                        Amount = 10,
                        Broker = broker1,
                        Category = technologyCategory,
                        DateBought = new DateTime(2021, 1, 1),
                        DateSold = new DateTime(2022, 1, 1)
                    },
                    new Asset
                    {
                        UserId = user1.Id,
                        User = user1,
                        AssetName = "Microsoft Corp.",
                        Ticker = "MSFT",
                        PriceBought = 200.0,
                        Amount = 10,
                        Broker = broker1,
                        Category = computersCategory,
                        DateBought = new DateTime(2021, 2, 1)
                    },
                    new Asset
                    {
                        UserId = user2.Id,
                        User = user2,
                        AssetName = "Amazon Inc.",
                        Ticker = "AMZN",
                        PriceBought = 2500.0,
                        Amount = 10,
                        Broker = broker2,
                        Category = ecommerceCategory,
                        DateBought = new DateTime(2022, 1, 1),
                        DateSold = new DateTime(2023, 1, 1)
                    },
                    new Asset
                    {
                        UserId = user3.Id,
                        User = user3,
                        AssetName = "Tesla Inc.",
                        Ticker = "TSLA",
                        PriceBought = 650.0,
                        Amount = 10,
                        Broker = broker3,
                        Category = carsCategory,
                        DateBought = new DateTime(2022, 6, 1)
                    },
                    new Asset
                    {
                        UserId = user2.Id,
                        User = user2,
                        AssetName = "Walmart Inc.",
                        Ticker = "WMT",
                        PriceBought = 140.0,
                        Amount = 10,
                        Broker = broker2,
                        Category = retailCategory,
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
