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
        public IEnumerable<Price> GetHistoricalPrice(string symbol, string fromDate, string toDate)
        {
            DateTime fromDateParsed = DateTime.Parse(fromDate);
            DateTime toDateParsed = DateTime.Parse(toDate);

            return _context.Prices.Where(pr => pr.Ticker == symbol && pr.Date <= fromDateParsed && pr.Date >= toDateParsed).AsEnumerable();
        }

        public bool AddPriceData(string symbol, IEnumerable<Price> priceData)
        {
            _context.Add(priceData);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
