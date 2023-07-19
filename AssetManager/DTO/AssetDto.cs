using AssetManager.Models;

namespace AssetManager.DTO
{
    public class AssetDto
    {
        public int UserId { get; set; }
        public string AssetName { get; set; }
        public string Ticker { get; set; }
        public double PriceBought { get; set; }
        public string BrokerName { get; set; }
        public DateTime DateBought { get; set; }
    }
}
