﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManager.Models
{
    public class Category : Classification
    {
        [ForeignKey("UserId")]

        public IdentityUser User { get; set; }

        public ICollection<Asset> Assets { get; set; }
    }
}
