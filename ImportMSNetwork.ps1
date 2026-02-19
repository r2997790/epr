# Import M&S UK Network Distribution Group Script
# This script imports the M&S UK Network from MSnetwork.xlsx into the database

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Import M&S UK Network Distribution Group" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Excel file exists
$excelFile = "MSnetwork.xlsx"
if (-not (Test-Path $excelFile)) {
    Write-Host "ERROR: Excel file not found: $excelFile" -ForegroundColor Red
    Write-Host "Please ensure MSnetwork.xlsx exists in the current directory." -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Found Excel file: $excelFile" -ForegroundColor Green
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "src\EPR.Web\EPR.Web.csproj")) {
    Write-Host "ERROR: EPR.Web project not found." -ForegroundColor Red
    Write-Host "Please ensure you're running this from the EPR root directory." -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Running import script..." -ForegroundColor Yellow
Write-Host ""

# Get full path to Excel file
$excelPath = (Resolve-Path $excelFile).Path

# Run the import command
$result = dotnet run --project src\EPR.Web -- seed-ms-network $excelPath

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "SUCCESS: M&S UK Network imported!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "The Distribution Group has been saved to the database." -ForegroundColor White
    Write-Host "You can now view it in the Visual Editor." -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "ERROR: Import failed" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Check the error messages above for details." -ForegroundColor Yellow
    Write-Host ""
}

Read-Host "Press Enter to exit"
















