using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using mobile_store_website1.Models;

namespace mobile_store_website1.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Model>? Model { get; set; }
        public DbSet<Order>? Order { get; set; }
        public DbSet<Product>? Product { get; set; }

    }
}