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
    public DbSet<CuisineType> CuisineTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Title = "Organic Bananas", Brand = "Nature's Best", Price = 2.99m, Category = "fruits", Thumbnail = "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Bananas" },
            new Product { Id = 2, Title = "Whole Milk", Brand = "Dairy Farm", Price = 3.49m, Category = "dairies", Thumbnail = "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Milk" },
            new Product { Id = 3, Title = "Chicken Breast", Brand = "Farm Fresh", Price = 8.99m, Category = "meats", Thumbnail = "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Chicken" },
            new Product { Id = 4, Title = "Tomato Sauce", Brand = "Chef's Choice", Price = 1.99m, Category = "sauces", Thumbnail = "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Tomato+Sauce" },
            new Product { Id = 5, Title = "Potato Chips", Brand = "SnackTime", Price = 4.50m, Category = "snacks", Thumbnail = "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Chips" },
            new Product { Id = 6, Title = "Fresh Broccoli", Brand = "Nature's Best", Price = 2.50m, Category = "vegetables", Thumbnail = "https://dummyimage.com/150x150/1e1e1e/ff5a00.png&text=Broccoli" }
        );
    }
}
