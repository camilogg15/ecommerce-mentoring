using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Infrastructure.Persistence
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category mapping
            modelBuilder.Entity<Category>(b =>
            {
                b.ToTable("Categories");
                b.HasKey(c => c.Id);
                b.Property(c => c.Name).IsRequired().HasMaxLength(50);
                b.Property(c => c.ImageUrl).HasMaxLength(200).IsUnicode(false);
                b.HasOne(c => c.ParentCategory)
                    .WithMany(c => c.Children)
                    .HasForeignKey(c => c.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Product mapping
            modelBuilder.Entity<Product>(b =>
            {
                b.ToTable("Products");
                b.HasKey(p => p.Id);
                b.Property(p => p.Id).ValueGeneratedOnAdd();
                b.Property(p => p.Name).IsRequired().HasMaxLength(50);
                b.Property(p => p.Description).HasColumnType("TEXT");
                b.Property(p => p.ImageUrl).HasMaxLength(200).IsUnicode(false);
                b.Property(p => p.Price).HasColumnType("REAL").IsRequired();
                b.Property(p => p.Amount).IsRequired();
                b.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
