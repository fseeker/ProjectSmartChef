using Microsoft.EntityFrameworkCore;
using Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework and SQLite
// IMPORTANT: Database MUST ALWAYS be in the Backend project folder, nowhere else
// Get the Backend project directory by looking for Backend.csproj
string projectRoot = AppContext.BaseDirectory;
// Navigate up until we find the directory containing Backend.csproj or reach Backend folder name
while (!File.Exists(Path.Combine(projectRoot, "Backend.csproj")) && !projectRoot.EndsWith("Backend"))
{
    var parent = Path.GetDirectoryName(projectRoot);
    if (parent == null || parent == projectRoot) break;
    projectRoot = parent;
}
// If we found Backend.csproj, use its directory; otherwise use the Backend folder
if (!File.Exists(Path.Combine(projectRoot, "Backend.csproj")))
{
    // Last resort: assume we're in Backend and just ensure it's the right place
    while (projectRoot.EndsWith("bin") || projectRoot.EndsWith("Debug") || projectRoot.EndsWith("Release") || projectRoot.EndsWith("net10.0"))
    {
        projectRoot = Path.GetDirectoryName(projectRoot) ?? projectRoot;
    }
}

var dbPath = Path.Combine(projectRoot, "smartchef.db");
Console.WriteLine($"Database path: {dbPath}");

var connectionString = $"Data Source={dbPath}";

builder.Services.AddDbContext<SmartChefContext>(options =>
    options.UseSqlite(connectionString));

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

// Apply migrations and ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SmartChefContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Serve frontend static files
// We search for index.html in the current or parent directories to handle different run environments (VS, dotnet run, etc.)
string? frontendDir = null;
var searchDir = new DirectoryInfo(projectRoot);
while (searchDir != null)
{
    if (File.Exists(Path.Combine(searchDir.FullName, "index.html")))
    {
        frontendDir = searchDir.FullName;
        break;
    }
    searchDir = searchDir.Parent;
}

if (frontendDir != null)
{
    // Define the static file path helper
    string GetPath(string file) => Path.Combine(frontendDir, file);

    app.MapGet("/", () => Results.File(GetPath("index.html"), "text/html"));
    app.MapGet("/script.js", () => Results.File(GetPath("script.js"), "application/javascript"));
    app.MapGet("/style.css", () => Results.File(GetPath("style.css"), "text/css"));

    // Add support for potential images or other assets in the same directory
    app.MapGet("/{*path}", (string path) => {
        var fullPath = GetPath(path);
        if (File.Exists(fullPath))
        {
            var extension = Path.GetExtension(path).ToLower();
            var contentType = extension switch {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".ico" => "image/x-icon",
                _ => "application/octet-stream"
            };
            return Results.File(fullPath, contentType);
        }
        return Results.NotFound();
    });
}
else
{
    Console.WriteLine("Warning: Could not find frontend files (index.html). Static file serving may fail.");
}

app.Run();
