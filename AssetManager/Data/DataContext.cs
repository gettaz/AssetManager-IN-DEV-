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
        public virtual DbSet<Broker> Brokers { get; set; }
        public virtual DbSet<Asset> Assets { get; set; }
        public virtual DbSet<Price> Prices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the relationship between Asset and Category
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust the delete behavior as needed

            // Define the relationship between Asset and Broker
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.Broker)
                .WithMany(b => b.Assets)
                .HasForeignKey(a => a.BrokerId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust the delete behavior as needed
        }
    }
}
