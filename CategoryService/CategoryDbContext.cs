using CategoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace CategoryService
{
    public class CategoryDbContext : DbContext
    {
        public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options)
        {}

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });
        }
    }
}
