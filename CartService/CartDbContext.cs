using CartService.Models;
using Microsoft.EntityFrameworkCore;

namespace CartService
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) 
        { }

        public DbSet<CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItem>()
                .HasIndex(x => new { x.UserId, x.ProductId })
                .IsUnique();
        }
    }
}
