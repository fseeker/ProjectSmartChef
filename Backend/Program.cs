using Microsoft.EntityFrameworkCore;
using Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework and SQLite
builder.Services.AddDbContext<SmartChefContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS to allow the frontend to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Serve frontend static files explicitly
var frontendDir = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, ".."));
app.MapGet("/", () => Results.File(Path.Combine(frontendDir, "index.html"), "text/html"));
app.MapGet("/script.js", () => Results.File(Path.Combine(frontendDir, "script.js"), "application/javascript"));
app.MapGet("/style.css", () => Results.File(Path.Combine(frontendDir, "style.css"), "text/css"));

app.Run();
