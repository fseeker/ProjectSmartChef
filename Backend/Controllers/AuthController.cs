using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using BCrypt.Net;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SmartChefContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(SmartChefContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || 
            string.IsNullOrWhiteSpace(request.Email) || 
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Username, Email, and Password are required." });
        }

        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return BadRequest(new { message = "Username already exists." });
        }
        
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already exists." });
        }

        var user = new User 
        { 
            Username = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { user.Id, user.Username, user.FullName, user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for: {UsernameOrEmail}", request.UsernameOrEmail);

        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
        {
            _logger.LogWarning("Login failed: missing username/email or password");
            return BadRequest(new { message = "Username/Email and Password are required." });
        }

        try
        {
            _logger.LogInformation("Searching for user by username: {UsernameOrEmail}", request.UsernameOrEmail);
            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Username == request.UsernameOrEmail);

            if (user == null)
            {
                _logger.LogInformation("User not found by username, searching by email: {UsernameOrEmail}", request.UsernameOrEmail);
                user = await _context.Users.FirstOrDefaultAsync(u => 
                    u.Email == request.UsernameOrEmail);
            }

            if (user == null)
            {
                _logger.LogWarning("User not found for: {UsernameOrEmail}", request.UsernameOrEmail);
                return Unauthorized(new { message = "Invalid credentials." });
            }

            _logger.LogInformation("User found: {UserId}, {Username}", user.Id, user.Username);
            _logger.LogInformation("PasswordHash empty: {IsEmpty}", string.IsNullOrEmpty(user.PasswordHash));

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                _logger.LogWarning("Password hash is empty for user: {UserId}", user.Id);
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            _logger.LogInformation("Password verification result: {IsValid}", passwordValid);

            if (!passwordValid)
            {
                _logger.LogWarning("Password verification failed for user: {UserId}", user.Id);
                return Unauthorized(new { message = "Invalid credentials." });
            }

            _logger.LogInformation("Login successful for user: {UserId}", user.Id);
            return Ok(new { user.Id, user.Username, user.FullName, user.Email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login for: {UsernameOrEmail}", request.UsernameOrEmail);
            return StatusCode(500, new { message = "An error occurred during login." });
        }
    }
}

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
