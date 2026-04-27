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
            .Include(u => u.Allergies)
            .Include(u => u.UserType)
            .Include(u => u.TastePalates)
                .ThenInclude(tp => tp.Taste)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound();
        
        return Ok(new { 
            user.Id, user.Username, user.FullName, user.Email,
            user.Bio, user.CookingSkill,
            UserTypeName = user.UserType?.Name ?? "Customer",
            FavoriteCuisines = user.FavoriteCuisines.Select(c => new { c.Id, c.Name }).ToList(),
            Allergies = user.Allergies.Select(a => new { a.Id, a.Title }).ToList(),
            TastePalates = user.TastePalates.Select(tp => new { 
                TasteId = tp.TasteId, 
                Name = tp.Taste.Name, 
                Rating = tp.Rating 
            }).ToList()
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UserProfileUpdate request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
        if (user == null) return NotFound();

        user.FullName = request.FullName ?? user.FullName;
        user.CookingSkill = request.CookingSkill ?? user.CookingSkill;
        user.Bio = request.Bio ?? user.Bio;

        await _context.SaveChangesAsync();
        return Ok(user);
    }
}

public class UserProfileUpdate
{
    public string? FullName { get; set; }
    public int? CookingSkill { get; set; }
    public string? Bio { get; set; }
}
