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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.Code)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<Coupon>()
                .HasMany(c => c.Items)
                .WithMany();

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithMany();

 
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Coupons)
                .WithMany();

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = UserRoles.Admin, Description = "Администратор системы" },
                new Role { Id = 2, Name = UserRoles.Manager, Description = "Менеджер" },
                new Role { Id = 3, Name = UserRoles.User, Description = "Пользователь" }
            );

            // учетка админа (пароль: admin123)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    Email = "admin@burgerking.com",
                    PasswordHash = "240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9",
                    RoleId = 1,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}