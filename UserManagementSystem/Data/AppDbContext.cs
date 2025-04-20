using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Models;

namespace UserManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        
        // Data/AppDbContext.cs
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id = 1, 
                    Username = "admin", 
                    Email = "admin@example.com", 
                    Password = "admin123", 
                    Phone = "0000000000", // 添加电话
                    IsActive = true 
                },
                new User { 
                    Id = 2, 
                    Username = "user1", 
                    Email = "user1@example.com", 
                    Password = "user123", 
                    Phone = "13800138000", 
                    IsActive = true 
                },
                new User { 
                    Id = 3, 
                    Username = "user2", 
                    Email = "user2@example.com", 
                    Password = "user123", 
                    Phone = "13900139000", 
                    IsActive = false 
                }
            );
        }
    }
}
