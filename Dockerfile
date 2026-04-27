# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore Backend/Backend.csproj
RUN dotnet publish Backend/Backend.csproj -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out ./

# Install sqlite3 for manual seeding
RUN apt-get update && apt-get install -y sqlite3 && rm -rf /var/lib/apt/lists/*

# Copy frontend files and seed scripts
COPY index.html style.css script.js ./
COPY seed/ ./seed/

# Create a dummy csproj so Program.cs identifies this as the project root
RUN touch Backend.csproj

ENTRYPOINT ["dotnet", "Backend.dll"]
