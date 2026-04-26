using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PantryController : ControllerBase
{
    private readonly SmartChefContext _context;

    public PantryController(SmartChefContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetPantryItems(int userId)
    {
        var items = await _context.PantryItems
            .Where(p => p.UserId == userId)
            .Select(p => p.Name)
            .ToListAsync();
            
        return Ok(items);
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> AddPantryItem(int userId, [FromBody] PantryRequest request)
    {
        Console.WriteLine($"Adding pantry item for user {userId}: {request.Name} ({request.Amount} {request.Unit})");
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Item name is required.");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) 
        {
            Console.WriteLine($"User {userId} not found for pantry addition.");
            return NotFound("User not found.");
        }

        var item = new PantryItem 
        { 
            Name = request.Name, 
            UserId = userId,
            Amount = request.Amount,
            Unit = request.Unit ?? ""
        };
        _context.PantryItems.Add(item);
        await _context.SaveChangesAsync();

        return Ok();
    }
}

public class PantryRequest
{
    public string Name { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string? Unit { get; set; }
}
