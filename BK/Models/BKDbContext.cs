using Microsoft.EntityFrameworkCore;

namespace BK.Models
{
    public class BKDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Order> Orders { get; set; }

        public BKDbContext(DbContextOptions<BKDbContext> options)
            : base(options) { }
    }
}
