# Seed Admin User Script
# This script creates the default admin user in the database

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Seed Admin User" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "src\EPR.Web\EPR.Web.csproj")) {
    Write-Host "ERROR: EPR.Web project not found." -ForegroundColor Red
    Write-Host "Please ensure you're running this from the EPR root directory." -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Running seed admin user script..." -ForegroundColor Yellow
Write-Host ""

# Run the seed command
$result = dotnet run --project src\EPR.Web -- seed-admin-user

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "SUCCESS: Admin user seeded!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Default credentials:" -ForegroundColor White
    Write-Host "  Username: admin" -ForegroundColor Cyan
    Write-Host "  Password: admin123" -ForegroundColor Cyan
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "ERROR: Seeding failed" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check the error messages above for details." -ForegroundColor Yellow
    Write-Host ""
}

Read-Host "Press Enter to exit"
















