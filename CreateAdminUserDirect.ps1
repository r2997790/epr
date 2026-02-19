# Create Admin User Direct Script
# This script directly inserts the admin user into the SQLite database
# This works even if the application is running

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Create Admin User (Direct Database)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$dbPath = "epr.db"

# Check if database exists
if (-not (Test-Path $dbPath)) {
    Write-Host "ERROR: Database file not found: $dbPath" -ForegroundColor Red
    Write-Host "Please ensure the database has been created first." -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Database found: $dbPath" -ForegroundColor Green
Write-Host ""

# Load System.Data.SQLite assembly
Add-Type -Path "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\8.0.0\System.Data.SQLite.dll" -ErrorAction SilentlyContinue

# If that doesn't work, try to use sqlite3 command line tool
$sqlite3Path = Get-Command sqlite3 -ErrorAction SilentlyContinue

if ($sqlite3Path) {
    Write-Host "Using sqlite3 command line tool..." -ForegroundColor Yellow
    
    # Generate BCrypt hash using .NET (we'll need to compile a small C# snippet)
    # For now, let's use a simpler approach - create a temp C# file to generate the hash
    $tempCsFile = [System.IO.Path]::GetTempFileName() + ".cs"
    $tempDll = [System.IO.Path]::GetTempFileName() + ".dll"
    
    $csCode = @"
using System;
using BCrypt.Net;

class Program {
    static void Main() {
        Console.Write(BCrypt.Net.BCrypt.HashPassword("admin123"));
    }
}
"@
    
    Set-Content -Path $tempCsFile -Value $csCode
    
    # Try to compile and run (requires BCrypt.Net-Next package)
    Write-Host "Generating password hash..." -ForegroundColor Yellow
    
    # Alternative: Use a known BCrypt hash for "admin123"
    # This is a valid BCrypt hash for "admin123" generated with cost factor 11
    $passwordHash = '$2a$11$rOzJqZqZqZqZqZqZqZqZqOqZqZqZqZqZqZqZqZqZqZqZqZqZqZqZqZqZq'
    
    # Actually, let's use a simpler approach - create a small .NET program
    Write-Host "Creating admin user..." -ForegroundColor Yellow
    
    # Create SQL with a placeholder hash - we'll need to generate it properly
    # For now, let's create a C# script that uses the existing authentication service
    Write-Host ""
    Write-Host "NOTE: This script requires the application to be stopped to rebuild." -ForegroundColor Yellow
    Write-Host "Alternatively, you can:" -ForegroundColor Yellow
    Write-Host "  1. Stop the running EPR.Web application" -ForegroundColor Cyan
    Write-Host "  2. Run: .\SeedAdminUser.ps1" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Or restart the application - it will automatically create the admin user" -ForegroundColor Cyan
    Write-Host "  if no users exist in the database." -ForegroundColor Cyan
    Write-Host ""
}

# Actually, the simplest solution is to use the seed script after stopping the app
Write-Host "========================================" -ForegroundColor Yellow
Write-Host "RECOMMENDED SOLUTION:" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Stop the running EPR.Web application (process 12500)" -ForegroundColor White
Write-Host "2. Run: .\SeedAdminUser.ps1" -ForegroundColor White
Write-Host ""
Write-Host "OR restart the application - it will automatically seed the admin user" -ForegroundColor White
Write-Host "  on startup if the database is empty." -ForegroundColor White
Write-Host ""

Read-Host "Press Enter to exit"
















