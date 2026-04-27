namespace Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public int CookingSkill { get; set; } // 1-10
    public List<CuisineType> FavoriteCuisines { get; set; } = new();
    public string Bio { get; set; } = string.Empty;

    // User Type (Customer, Admin, Staff)
    public int UserTypeId { get; set; }
    public UserType? UserType { get; set; }

    // Relationships
    public List<PantryItem> PantryItems { get; set; } = new();
    public List<Product> Allergies { get; set; } = new();
    public List<UserTastePalate> TastePalates { get; set; } = new();
}
