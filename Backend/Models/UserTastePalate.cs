namespace Backend.Models;

public class UserTastePalate
{
    public int UserId { get; set; }
    public User? User { get; set; }
    
    public int TasteId { get; set; }
    public TastePalate? Taste { get; set; }
    
    public int Rating { get; set; }
}
