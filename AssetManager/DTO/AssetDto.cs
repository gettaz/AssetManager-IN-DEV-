using AssetManager.Models;

namespace AssetManager.DTO
{
    public class AssetDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? AssetName { get; set; }
        public string? Ticker { get; set; }
        public double? PurchasePrice { get; set; }
        public string? BrokerName { get; set; }
        public DateTime? DateBought { get; set; }
        public double? Amount { get; set; }
        public string? CategoryName { get; set; }
    }
}