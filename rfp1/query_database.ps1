# Query Empauer Database
# Run this script to check if data was written to the database

$dbPath = "C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web\empauer.db"

Write-Host "=== Empauer Database Query Tool ===" -ForegroundColor Cyan
Write-Host "Database: $dbPath" -ForegroundColor Gray
Write-Host ""

if (-not (Test-Path $dbPath)) {
    Write-Host "ERROR: Database file not found!" -ForegroundColor Red
    Write-Host "Expected location: $dbPath" -ForegroundColor Yellow
    exit 1
}

# Check if sqlite3 is available
$sqlite3 = Get-Command sqlite3 -ErrorAction SilentlyContinue

if (-not $sqlite3) {
    Write-Host "SQLite3 command-line tool not found." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Option 1: Install SQLite3" -ForegroundColor Cyan
    Write-Host "  Download from: https://www.sqlite.org/download.html" -ForegroundColor Gray
    Write-Host "  Or use Chocolatey: choco install sqlite" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 2: Use DB Browser for SQLite (GUI)" -ForegroundColor Cyan
    Write-Host "  Download: https://sqlitebrowser.org/" -ForegroundColor Gray
    Write-Host "  Then open: $dbPath" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Option 3: Use dotnet-sqlite tool" -ForegroundColor Cyan
    Write-Host "  Run: dotnet tool install --global dotnet-sqlite" -ForegroundColor Gray
    Write-Host ""
    
    # Try using dotnet-sqlite if available
    $dotnetSqlite = Get-Command dotnet-sqlite -ErrorAction SilentlyContinue
    if ($dotnetSqlite) {
        Write-Host "Using dotnet-sqlite..." -ForegroundColor Green
        $sqlite3 = "dotnet-sqlite"
    } else {
        Write-Host "No SQLite tools found. Please install one of the options above." -ForegroundColor Red
        exit 1
    }
}

Write-Host "=== ASSESSMENTS TABLE ===" -ForegroundColor Yellow
Write-Host ""
Write-Host "Total Assessments:" -ForegroundColor Cyan
& $sqlite3.Name $dbPath "SELECT COUNT(*) as count FROM Assessments;"
Write-Host ""

Write-Host "All Assessments (Code, Description, Type, Status, Created Date):" -ForegroundColor Cyan
& $sqlite3.Name $dbPath -header -column "SELECT Code, Description, TypeCode, Status, CompanyName, datetime(CreatedDate, 'localtime') as CreatedDate FROM Assessments ORDER BY CreatedDate DESC LIMIT 20;"
Write-Host ""

Write-Host "=== LIFECYCLE STAGES ===" -ForegroundColor Yellow
Write-Host ""
& $sqlite3.Name $dbPath -header -column "SELECT AssessmentCode, Title, Visible, SortOrder FROM AssessmentLifecycleStages ORDER BY AssessmentCode, SortOrder LIMIT 50;"
Write-Host ""

Write-Host "=== RECENT ACTIVITY LOGS ===" -ForegroundColor Yellow
Write-Host ""
& $sqlite3.Name $dbPath -header -column "SELECT Id, UserId, Action, EntityType, EntityId, datetime(Timestamp, 'localtime') as Timestamp FROM ActivityLogs WHERE Action LIKE '%Assessment%' ORDER BY Timestamp DESC LIMIT 10;"
Write-Host ""

Write-Host "=== MATERIAL INPUTS ===" -ForegroundColor Yellow
Write-Host ""
Write-Host "Count:" -ForegroundColor Cyan
& $sqlite3.Name $dbPath "SELECT COUNT(*) as count FROM MaterialInputs;"
Write-Host ""

Write-Host "Sample Data:" -ForegroundColor Cyan
& $sqlite3.Name $dbPath -header -column "SELECT AssessmentCode, Category, ProductCoProduct, Mass, Cost FROM MaterialInputs LIMIT 10;"
Write-Host ""

Write-Host "=== DONE ===" -ForegroundColor Green
Write-Host ""
Write-Host "To query specific assessment, run:" -ForegroundColor Gray
Write-Host "  sqlite3 `"$dbPath`" `"SELECT * FROM Assessments WHERE Code = 'ASMT-001';`"" -ForegroundColor Gray




