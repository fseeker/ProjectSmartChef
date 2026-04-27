using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class SmartChefContext : DbContext
{
    public SmartChefContext(DbContextOptions<SmartChefContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<PantryItem> PantryItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<CuisineType> CuisineTypes { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<UserType> UserTypes { get; set; }
    public DbSet<TastePalate> TastePalates { get; set; }
    public DbSet<UserTastePalate> UserTastePalates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite Key for UserTastePalate
        modelBuilder.Entity<UserTastePalate>()
            .HasKey(ut => new { ut.UserId, ut.TasteId });

        // One-to-Many: User -> Orders
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId);

        // Many-to-Many: Order <-> Product through OrderProduct
        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => new { op.OrderId, op.ProductId });

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderProducts)
            .HasForeignKey(op => op.OrderId);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Product)
            .WithMany()
            .HasForeignKey(op => op.ProductId);

        // Many-to-Many: User <-> CuisineType -> table "UserPreferredCuisines"
        modelBuilder.Entity<User>()
            .HasMany(u => u.FavoriteCuisines)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserPreferredCuisines"));

        // Many-to-Many: User <-> Product (Allergies) -> table "UserAllergies"
        modelBuilder.Entity<User>()
            .HasMany(u => u.Allergies)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserAllergies"));

        // Map decimals to REAL/decimal in SQLite
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<PantryItem>()
            .Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderProduct>()
            .Property(op => op.Amount)
            .HasColumnType("decimal(18,2)");
    }
}
