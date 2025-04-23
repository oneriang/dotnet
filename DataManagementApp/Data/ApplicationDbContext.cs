using DataManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DataManagementApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
        }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Product>(entity =>
        //     {
        //         entity.ToTable("Products");
        //         entity.HasKey(e => e.Id);
                
        //         entity.Property(e => e.Name)
        //             .IsRequired()
        //             .HasMaxLength(100);
                    
        //         entity.Property(e => e.Price)
        //             .HasColumnType("REAL")
        //             .IsRequired();
                    
        //         entity.Property(e => e.StockQuantity)
        //             .IsRequired();
                    
        //         entity.Property(e => e.CreatedAt)
        //             .IsRequired()
        //             .HasColumnType("TEXT");
                    
        //         entity.Property(e => e.UpdatedAt)
        //             .HasColumnType("TEXT");
                    
        //         entity.Property(e => e.IsDeleted)
        //             .IsRequired()
        //             .HasDefaultValue(false);
        //     });
        // }
    }
}
