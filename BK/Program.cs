using BK.Mappings;
using BK.Models;
using BK.Repositories;
using BK.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BKDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnection")));

// НОВАЯ НАСТРОЙКА AUTOMAPPER
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BKDbContext>();

    context.Database.EnsureCreated();

    if (!context.Roles.Any())
    {
        Console.WriteLine("Создание ролей...");

        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "Admin", Description = "Администратор системы" },
            new Role { Id = 2, Name = "Manager", Description = "Менеджер" },
            new Role { Id = 3, Name = "User", Description = "Пользователь" }
        };

        context.Roles.AddRange(roles);
        context.SaveChanges();
        Console.WriteLine("Роли успешно созданы");
    }

    if (!context.Users.Any(u => u.RoleId == 1))
    {
        Console.WriteLine("Создание администратора...");

        var adminUser = new User
        {
            Login = "admin",
            Email = "admin@burgerking.com",
            PasswordHash = HashPassword("admin123"),
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
        Console.WriteLine("Администратор создан: admin / admin123");
    }

    if (!context.Users.Any(u => u.RoleId == 2))
    {
        Console.WriteLine("Создание менеджера...");

        var adminUser = new User
        {
            Login = "ms",
            Email = "ms@burgerking.com",
            PasswordHash = HashPassword("ms123"),
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
        Console.WriteLine("Администратор создан: admin / admin123");
        Console.WriteLine("Менеджер создан: ms / ms123");
    }
}




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

string HashPassword(string password)
{
    using var sha256 = System.Security.Cryptography.SHA256.Create();
    var bytes = System.Text.Encoding.UTF8.GetBytes(password);
    var hash = sha256.ComputeHash(bytes);
    return Convert.ToBase64String(hash);
}