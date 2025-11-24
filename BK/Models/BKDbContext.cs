using Microsoft.EntityFrameworkCore;

namespace BK.Models
{
    public class BKDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public BKDbContext(DbContextOptions<BKDbContext> options) : base(options) { }

        
    }
}