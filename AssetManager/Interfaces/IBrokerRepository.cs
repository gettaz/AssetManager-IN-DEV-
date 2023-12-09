using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface IBrokerRepository
    {
        //TODO: search by name in sql- instead of get all
        IEnumerable<Broker> GetUserBrokers(string userId);
        bool CreateBroker(Broker broker);
        bool UpdateBroker(Broker broker);
        bool DeleteBroker(string userId, int brokerId);
        bool Save();
    }
}
