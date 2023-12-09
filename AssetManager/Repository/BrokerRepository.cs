using AssetManager.Data;
using AssetManager.Interfaces;
using AssetManager.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetManager.Repository
{
    public class BrokerRepository : IBrokerRepository
    {
        private readonly DataContext _context;

        public BrokerRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateBroker(Broker broker)
        {
            _context.Brokers.Add(broker);
            return Save();
        }

        public bool DeleteBroker(string userId, int brokerId)
        {
            var broker = _context.Brokers.FirstOrDefault(b => b.Id == brokerId && b.UserId == userId);

            if (broker == null)
            {
                return false;
            }

            _context.Brokers.Remove(broker);
            return Save();
        }

        public IEnumerable<Broker> GetUserBrokers(string userId)
        {
            return _context.Brokers
                .Where(b => b.UserId == userId).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool UpdateBroker(Broker broker)
        {
            _context.Brokers.Update(broker);
            return Save();
        }
    }
}
