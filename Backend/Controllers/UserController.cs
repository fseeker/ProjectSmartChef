using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly SmartChefContext _context;

    public UserController(SmartChefContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        var user = await _context.Users
            .Include(u => u.FavoriteCuisines)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();
        
        return Ok(new { 
            user.Id, user.Username, user.FullName, user.Email, 
            user.DietaryPreferences, 
            FavoriteCuisines = string.Join(", ", user.FavoriteCuisines.Select(c => c.Name)),
            user.Bio 
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UserProfileUpdate request)
    {
        Console.WriteLine($"Updating profile for user {id}. Name: {request.FullName}, Bio: {request.Bio}");
        var user = await _context.Users
            .Include(u => u.FavoriteCuisines)
            .FirstOrDefaultAsync(u => u.Id == id);
            
        if (user == null) 
        {
            Console.WriteLine($"User {id} not found!");
            return NotFound();
        }

        user.FullName = request.FullName ?? user.FullName;
        user.DietaryPreferences = request.DietaryPreferences ?? user.DietaryPreferences;
        user.Bio = request.Bio ?? user.Bio;

        if (request.FavoriteCuisines != null)
        {
            // Clear existing and re-add
            user.FavoriteCuisines.Clear();
            var cuisines = request.FavoriteCuisines.Split(',')
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => new CuisineType { Name = c })
                .ToList();
            
            user.FavoriteCuisines.AddRange(cuisines);
        }

        await _context.SaveChangesAsync();
        return Ok(user);
    }
}

public class UserProfileUpdate
{
    public string? FullName { get; set; }
    public string? DietaryPreferences { get; set; }
    public string? FavoriteCuisines { get; set; }
    public string? Bio { get; set; }
}
