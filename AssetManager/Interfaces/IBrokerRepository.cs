﻿using AssetManager.DTO;
using AssetManager.Models;

namespace AssetManager.Interfaces
{
    public interface IBrokerRepository
    {
        //TODO: search by name in sql- instead of get all
        IEnumerable<Broker> GetUserBrokers(string userId);
        public IEnumerable<ClassificationAssetCount> GetBrokersAssetCount(string userId);
        bool CreateBroker(Broker broker);
        bool UpdateBroker(Broker broker);
        bool DeleteBroker(string userId, int brokerId);
        int GetBrokerId(string userId, string? name);

        bool Save();
    }
}
