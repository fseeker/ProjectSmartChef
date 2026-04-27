using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly SmartChefContext _context;

    public ProductsController(SmartChefContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string category = "all")
    {
        var query = _context.Products.Where(p => p.IsVisible == true);

        if (category.ToLower() != "all")
        {
            query = query.Where(p => p.Category.ToLower() == category.ToLower());
        }

        var products = await query.Take(60)
            .Include(p => p.Unit)
            .Select(p => new {
                p.Id,
                p.Title,
                p.Category,
                p.Brand,
                p.Price,
                p.Thumbnail,
                p.Calories,
                p.AllowedAmounts,
                p.UnitId,
                UnitName = p.Unit != null ? p.Unit.Name : ""
            })
            .ToListAsync();
        return Ok(products);
    }
}
