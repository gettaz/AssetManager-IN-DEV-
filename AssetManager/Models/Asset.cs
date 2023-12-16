using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManager.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string AssetName { get; set; }
        public string Ticker { get; set; }
        public double PriceBought { get; set; }
        public double Amount { get; set; }
        public DateTime DateBought { get; set; }
        public DateTime? DateSold { get; set; }
        public int? CategoryId { get; set; }
        public int? BrokerId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [ForeignKey("BrokerId")]
        public Broker? Broker { get; set; }

        [Required]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
