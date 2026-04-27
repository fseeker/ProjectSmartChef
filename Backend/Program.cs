using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Backend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// 1. DYNAMIC DATABASE PATHING
// Ensure the DB is ALWAYS in the Backend project folder
string projectRoot = AppContext.BaseDirectory;
while (!File.Exists(Path.Combine(projectRoot, "Backend.csproj")) && !projectRoot.EndsWith("Backend"))
{
    var parent = Path.GetDirectoryName(projectRoot);
    if (parent == null || parent == projectRoot) break;
    projectRoot = parent;
}
if (!File.Exists(Path.Combine(projectRoot, "Backend.csproj")))
{
    // If not found by name, assume we are in a subfolder of Backend (like bin/Debug)
    var current = AppContext.BaseDirectory;
    while (!File.Exists(Path.Combine(current, "Backend.csproj")) && current != Path.GetPathRoot(current))
    {
        current = Path.GetDirectoryName(current) ?? current;
    }
    projectRoot = current;
}

var dbPath = Path.Combine(projectRoot, "smartchef.db");
Console.WriteLine($"[INIT] Database Location: {dbPath}");

builder.Services.AddDbContext<SmartChefContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// 2. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// 3. FRONTEND PATH DISCOVERY
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

var app = builder.Build();

// 3. DATABASE MIGRATIONS & SEEDING
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SmartChefContext>();
    dbContext.Database.Migrate();

    // AUTO-SEEDING (All .sql files in seed folder)
    if (!dbContext.Set<Backend.Models.Product>().Any())
    {
        string seedDir = Path.Combine(frontendDir ?? projectRoot, "seed");
        if (Directory.Exists(seedDir))
        {
            var seedFiles = Directory.GetFiles(seedDir, "*.sql").OrderBy(f => f);
            foreach (var file in seedFiles)
            {
                try
                {
                    Console.WriteLine($"[INIT] Executing seed script: {Path.GetFileName(file)}");
                    var sql = File.ReadAllText(file);
                    dbContext.Database.ExecuteSqlRaw(sql);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[INIT] Seeding failed for {Path.GetFileName(file)}: {ex.Message}");
                }
            }
            Console.WriteLine("[INIT] All seeding operations complete.");
        }
        else
        {
            Console.WriteLine($"[INIT] Seed directory not found at: {seedDir}");
        }
    }
}

// 4. PIPELINE
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// 5. CLEAN STATIC FILE SERVING
if (frontendDir != null)
{
    Console.WriteLine($"[INIT] Serving Frontend from: {frontendDir}");
    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = new PhysicalFileProvider(frontendDir),
        RequestPath = ""
    });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendDir),
        RequestPath = ""
    });
}
else
{
    Console.WriteLine("[INIT] Warning: index.html not found. Frontend serving disabled.");
}

app.Run();
