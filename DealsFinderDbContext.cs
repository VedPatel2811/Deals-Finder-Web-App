using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5
{
    public class DealsFinderDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<FoodDeliveryService> FoodDeliveryServices { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        public DealsFinderDbContext(DbContextOptions<DealsFinderDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<FoodDeliveryService>().ToTable("FoodDeliveryService");
            modelBuilder.Entity<Subscription>().ToTable("Subscription");

            modelBuilder.Entity<Subscription>()
                .HasKey(b => new { b.CustomerId, b.ServiceId });
        }
    }
}
