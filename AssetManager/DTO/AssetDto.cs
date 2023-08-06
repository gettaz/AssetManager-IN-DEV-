using AssetManager.Models;

namespace AssetManager.DTO
{
    public class AssetDto
    {
        public string UserId { get; set; }
        public int? Id { get; set; }
        public string? AssetName { get; set; }
        public string? Ticker { get; set; }
        public double? PriceBought { get; set; }
        public string? BrokerName { get; set; }
        public DateTime? DateBought { get; set; }
        public double? Amount { get; set; }
    }
}
