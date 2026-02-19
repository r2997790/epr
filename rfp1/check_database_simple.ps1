# Simple PowerShell commands to check database
# Run these commands one by one in PowerShell

# 1. Navigate to the project directory (adjust path as needed)
cd C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web

# 2. Check if database exists
if (Test-Path "empauer.db") {
    Write-Host "Database found!" -ForegroundColor Green
    
    # 3. If you have sqlite3 installed, run these queries:
    # Count assessments
    sqlite3 empauer.db "SELECT COUNT(*) as 'Total Assessments' FROM Assessments;"
    
    # List all assessments
    sqlite3 empauer.db -header -column "SELECT Code, Description, TypeCode, Status, CompanyName, CreatedDate FROM Assessments ORDER BY CreatedDate DESC;"
    
    # Check lifecycle stages
    sqlite3 empauer.db -header -column "SELECT AssessmentCode, Title FROM AssessmentLifecycleStages ORDER BY AssessmentCode;"
    
} else {
    Write-Host "Database not found at: empauer.db" -ForegroundColor Red
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
}




