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
            .Include(p => p.Unit)
            .Include(p => p.Product)
            .Where(p => p.UserId == userId)
            .Select(p => new {
                p.Id,
                Name = p.Name ?? p.Product!.Title,
                p.ProductId,
                p.Amount,
                p.UnitId,
                UnitName = p.Unit != null ? p.Unit.Name : ""
            })
            .ToListAsync();
            
        return Ok(items);
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> AddPantryItem(int userId, [FromBody] PantryRequest request)
    {
        Console.WriteLine($"Adding pantry item for user {userId}: {request.Name ?? request.ProductId?.ToString()} ({request.Amount} UnitId: {request.UnitId})");
        if (string.IsNullOrWhiteSpace(request.Name) && request.ProductId == null)
            return BadRequest("Item name or ProductId is required.");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) 
        {
            Console.WriteLine($"User {userId} not found for pantry addition.");
            return NotFound("User not found.");
        }

        var item = new PantryItem 
        { 
            Name = request.Name, 
            ProductId = request.ProductId,
            UserId = userId,
            Amount = request.Amount,
            UnitId = request.UnitId
        };
        _context.PantryItems.Add(item);
        await _context.SaveChangesAsync();

        return Ok();
    }
}

public class PantryRequest
{
    public string? Name { get; set; }
    public int? ProductId { get; set; }
    public decimal Amount { get; set; }
    public int UnitId { get; set; }
}
