namespace Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<PantryItem> PantryItems { get; set; } = new();
}
