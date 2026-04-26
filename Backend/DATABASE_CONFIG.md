# Database Configuration - FIXED

## Database Location
- **ONLY Location**: `C:\Users\fahim\Downloads\ProjectSmartChef\projectPath\ProjectSmartChef\Backend\smartchef.db`
- **This is CONSTANT** - No other database files will be created anywhere else

## How It Works
1. **Program.cs** uses Backend.csproj location to determine the Backend folder
2. Database is ALWAYS created at: `{BackendFolder}/smartchef.db`
3. **Same database file** whether running from:
   - Visual Studio (F5 debug)
   - `dotnet run` from terminal
   - Direct exe execution

## Key Implementation Details
```csharp
// Finds Backend.csproj file to locate the exact Backend folder
string projectRoot = AppContext.BaseDirectory;
while (!File.Exists(Path.Combine(projectRoot, "Backend.csproj")))
{
    projectRoot = Path.GetDirectoryName(projectRoot) ?? projectRoot;
}

// Database ALWAYS here
var dbPath = Path.Combine(projectRoot, "smartchef.db");
```

## Verified
✅ Only ONE smartchef.db file exists (in Backend folder)
✅ No bin/ or Release/ database copies
✅ .gitignore prevents stray files
✅ Console logs database path on startup
✅ Migrations applied automatically
