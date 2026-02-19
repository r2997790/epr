# PowerShell script to check database contents
# Run this from the project root or EmpauerLocal.Web directory

Write-Host "=== Checking Empauer Database ===" -ForegroundColor Cyan
Write-Host ""

# Find the database file
$dbPath = "empauer.db"
if (Test-Path $dbPath) {
    Write-Host "✓ Database file found: $dbPath" -ForegroundColor Green
    $fileInfo = Get-Item $dbPath
    Write-Host "  Size: $($fileInfo.Length) bytes" -ForegroundColor Gray
    Write-Host "  Last Modified: $($fileInfo.LastWriteTime)" -ForegroundColor Gray
    Write-Host ""
    
    # Check if sqlite3 is available
    $sqlite3 = Get-Command sqlite3 -ErrorAction SilentlyContinue
    if ($sqlite3) {
        Write-Host "=== Querying Assessments ===" -ForegroundColor Cyan
        Write-Host ""
        
        # Count assessments
        $count = sqlite3 $dbPath "SELECT COUNT(*) FROM Assessments;"
        Write-Host "Total Assessments: $count" -ForegroundColor Yellow
        Write-Host ""
        
        # List all assessments
        Write-Host "=== All Assessments ===" -ForegroundColor Cyan
        sqlite3 $dbPath -header -column "SELECT Code, Description, TypeCode, Status, CompanyName, CreatedDate FROM Assessments ORDER BY CreatedDate DESC LIMIT 20;"
        Write-Host ""
        
        # List lifecycle stages
        Write-Host "=== Lifecycle Stages ===" -ForegroundColor Cyan
        sqlite3 $dbPath -header -column "SELECT AssessmentCode, Title, Visible, SortOrder FROM AssessmentLifecycleStages ORDER BY AssessmentCode, SortOrder LIMIT 50;"
        Write-Host ""
        
        # Check recent activity
        Write-Host "=== Recent Activity Logs ===" -ForegroundColor Cyan
        sqlite3 $dbPath -header -column "SELECT Id, UserId, Action, EntityType, EntityId, Timestamp FROM ActivityLogs ORDER BY Timestamp DESC LIMIT 10;"
        Write-Host ""
        
    } else {
        Write-Host "⚠ SQLite3 command-line tool not found." -ForegroundColor Yellow
        Write-Host "  Install it from: https://www.sqlite.org/download.html" -ForegroundColor Gray
        Write-Host "  Or use: dotnet tool install --global dotnet-sqlite" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Alternative: Use a SQLite browser like DB Browser for SQLite" -ForegroundColor Yellow
        Write-Host "  Download: https://sqlitebrowser.org/" -ForegroundColor Gray
    }
} else {
    Write-Host "✗ Database file not found at: $dbPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "Searching in common locations..." -ForegroundColor Yellow
    
    $searchPaths = @(
        ".\empauer.db",
        ".\EmpauerLocal\src\EmpauerLocal.Web\empauer.db",
        ".\EmpauerLocal.Web\empauer.db",
        "$env:USERPROFILE\empauer.db"
    )
    
    foreach ($path in $searchPaths) {
        if (Test-Path $path) {
            Write-Host "  Found at: $path" -ForegroundColor Green
            $dbPath = $path
            break
        }
    }
    
    if (-not (Test-Path $dbPath)) {
        Write-Host "  Database file not found in any common location." -ForegroundColor Red
        Write-Host "  The database will be created automatically when the application starts." -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "=== Done ===" -ForegroundColor Cyan
