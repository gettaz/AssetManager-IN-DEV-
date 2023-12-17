using AssetManager.Data;
using AssetManager.DTO;
using AssetManager.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetManager.Repository
{
    public class PriceRepository : IPriceRepository
    {
        private readonly DataContext _context;
        public PriceRepository(DataContext context)
        {
            _context = context;
        }
        public IEnumerable<Price> GetHistoricalPrice(string symbol, DateTime fromDate, DateTime toDate)
        {
            var prices = _context.Prices.Where(pr => pr.Ticker == symbol && pr.Date >= fromDate && pr.Date <= toDate);
            return prices.ToList();
        }

        public bool AddPriceData(string symbol, IEnumerable<Price> priceData)
        {
            _context.AddRange(priceData);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
