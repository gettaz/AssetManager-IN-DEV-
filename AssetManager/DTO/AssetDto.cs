using System.ComponentModel.DataAnnotations;

namespace AssetManager.DTO
{
    public class AssetDto
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Asset name is required")]
        public string? AssetName { get; set; }

        [Required(ErrorMessage = "Ticker is required")]
        public string? Ticker { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Purchase price must be above 0")]
        public double? PurchasePrice { get; set; }
        public string? BrokerName { get; set; } = "none";
        public DateTime? DateBought { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Amount must be above 0")]
        public double? Amount { get; set; }

        public string? CategoryName { get; set; } = "none";
    }
}