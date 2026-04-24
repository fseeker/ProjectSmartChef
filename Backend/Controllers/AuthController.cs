using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SmartChefContext _context;

    public AuthController(SmartChefContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username is required.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        
        if (user == null)
        {
            user = new User { Username = request.Username };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        return Ok(new { user.Id, user.Username });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
}
