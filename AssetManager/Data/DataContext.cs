using AssetManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AssetManager.Data
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<AssetCategory> AssetsCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AssetCategory>()
                .HasKey(ac => new { ac.AssetId, ac.CategoryId });

            modelBuilder.Entity<AssetCategory>()
                .HasOne(a => a.Asset)
                .WithMany(ac => ac.AssetCategories)
                .HasForeignKey(ac => ac.AssetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssetCategory>()
                .HasOne(c => c.Category)
                .WithMany(c => c.AssetCategories)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}