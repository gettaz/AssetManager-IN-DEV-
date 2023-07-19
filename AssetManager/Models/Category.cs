﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManager.Models
{
    public class Category 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        public ICollection<AssetCategory> AssetCategories { get; set; }
    }
}