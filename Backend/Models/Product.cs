namespace Backend.Models;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Thumbnail { get; set; } = string.Empty;
    public int Calories { get; set; }
    public string AllowedAmounts { get; set; } = string.Empty;
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    public bool IsVisible { get; set; } = true;
}
