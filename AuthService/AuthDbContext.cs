using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthService
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "admin-role-id",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "customer-role-id",
                    Name = "Customer",
                    NormalizedName = "CUSTOMER"
                }
            );
        }
    }
}