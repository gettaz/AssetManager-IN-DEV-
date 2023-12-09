using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManager.Models
{
    public class Broker : Classification
    {
        [ForeignKey("UserId")]

        public IdentityUser User { get; set; }

        public ICollection<Asset> Assets { get; set; }
    }
}