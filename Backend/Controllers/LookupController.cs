using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupController : ControllerBase
{
    private readonly SmartChefContext _context;

    public LookupController(SmartChefContext context)
    {
        _context = context;
    }

    [HttpGet("cuisines")]
    public async Task<IActionResult> GetCuisines()
    {
        return Ok(await _context.CuisineTypes.ToListAsync());
    }

    [HttpGet("tastes")]
    public async Task<IActionResult> GetTastes()
    {
        return Ok(await _context.TastePalates.ToListAsync());
    }

    [HttpGet("usertypes")]
    public async Task<IActionResult> GetUserTypes()
    {
        return Ok(await _context.UserTypes.ToListAsync());
    }

    [HttpGet("units")]
    public async Task<IActionResult> GetUnits()
    {
        return Ok(await _context.Units.ToListAsync());
    }
}
