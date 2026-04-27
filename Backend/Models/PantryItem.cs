namespace Backend.Models;

public class PantryItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    
    // Optional reference to a catalog product
    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    public decimal Amount { get; set; }
    
    // Reference to Unit
    public int UnitId { get; set; }
    public Unit? Unit { get; set; }
    
    // Foreign key to User
    public int UserId { get; set; }
    public User? User { get; set; }
}
