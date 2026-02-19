# Quick Database Check Commands

## Database Location
```
C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web\empauer.db
```

## Option 1: Using PowerShell Script (Easiest)

Run this command in PowerShell:
```powershell
cd C:\Users\Ryan\Desktop\empauer
.\query_database.ps1
```

## Option 2: Using SQLite3 Command Line

If you have SQLite3 installed, run these commands:

```powershell
cd C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web

# Count assessments
sqlite3 empauer.db "SELECT COUNT(*) FROM Assessments;"

# List all assessments
sqlite3 empauer.db -header -column "SELECT Code, Description, TypeCode, Status, CompanyName, datetime(CreatedDate, 'localtime') as CreatedDate FROM Assessments ORDER BY CreatedDate DESC;"

# Check lifecycle stages
sqlite3 empauer.db -header -column "SELECT AssessmentCode, Title FROM AssessmentLifecycleStages ORDER BY AssessmentCode;"

# Check recent activity
sqlite3 empauer.db -header -column "SELECT Action, EntityType, EntityId, datetime(Timestamp, 'localtime') as Timestamp FROM ActivityLogs WHERE Action LIKE '%Assessment%' ORDER BY Timestamp DESC LIMIT 10;"
```

## Option 3: Using DB Browser for SQLite (GUI - Recommended)

1. Download DB Browser for SQLite: https://sqlitebrowser.org/
2. Open the database file: `C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web\empauer.db`
3. Go to "Browse Data" tab
4. Select "Assessments" table to see all assessments

## Option 4: Install SQLite3 (if not installed)

### Using Chocolatey:
```powershell
choco install sqlite
```

### Using Scoop:
```powershell
scoop install sqlite
```

### Manual Download:
Download from: https://www.sqlite.org/download.html
Extract and add to PATH

## Quick One-Liner Commands

### Check if database exists and size:
```powershell
cd C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web; if (Test-Path empauer.db) { Write-Host "Found! Size: $((Get-Item empauer.db).Length) bytes" } else { Write-Host "Not found!" }
```

### Count assessments (if sqlite3 installed):
```powershell
cd C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web; sqlite3 empauer.db "SELECT COUNT(*) FROM Assessments;"
```

### List all assessments:
```powershell
cd C:\Users\Ryan\Desktop\empauer\EmpauerLocal\src\EmpauerLocal.Web; sqlite3 empauer.db -header -column "SELECT Code, Description, TypeCode, Status, CreatedDate FROM Assessments ORDER BY CreatedDate DESC;"
```

## What to Look For

1. **Assessments Table**: Should contain all created assessments with:
   - Code (e.g., ASMT-001, ASMT-002)
   - Description (assessment name)
   - TypeCode (e.g., "Food Product")
   - Status (e.g., "DRAFT")
   - CreatedDate (timestamp)

2. **AssessmentLifecycleStages Table**: Should contain lifecycle stages for each assessment

3. **ActivityLogs Table**: Should contain log entries when assessments are created

## Troubleshooting

If you see 0 assessments:
- Check if the application is running
- Check server logs for errors
- Verify the database file is being written to
- Check if there are any transaction rollbacks

If assessments exist but don't appear in UI:
- Check cache invalidation
- Verify GetAssessments() query
- Check for filtering logic




