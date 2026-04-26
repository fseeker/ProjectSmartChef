namespace Backend.Models;

public class PantryItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Unit { get; set; } 
    
    // Foreign key to User
    public int UserId { get; set; }
    public User? User { get; set; }
}
