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

        [ForeignKey("User")]
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

    }
}