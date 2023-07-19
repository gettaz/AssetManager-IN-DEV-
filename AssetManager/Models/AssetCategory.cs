using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManager.Models
{
    public class AssetCategory
    {
        public int AssetId { get; set; }
        public int CategoryId { get; set; }
        public Asset Asset { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

    }
}