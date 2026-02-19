@echo off
setlocal

echo ========================================
echo Import M&S UK Network Distribution Group
echo ========================================
echo.

:: Check if Excel file exists
set "EXCEL_FILE=MSnetwork.xlsx"
if not exist "%EXCEL_FILE%" (
    echo ERROR: Excel file not found: %EXCEL_FILE%
    echo Please ensure MSnetwork.xlsx exists in the current directory.
    echo.
    pause
    exit /b 1
)

echo Found Excel file: %EXCEL_FILE%
echo.

:: Check if we're in the right directory (should have src/EPR.Web)
if not exist "src\EPR.Web\EPR.Web.csproj" (
    echo ERROR: EPR.Web project not found.
    echo Please ensure you're running this from the EPR root directory.
    echo.
    pause
    exit /b 1
)

echo Running import script...
echo.

:: Run the import command
dotnet run --project src\EPR.Web -- seed-ms-network "%CD%\%EXCEL_FILE%"

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo SUCCESS: M&S UK Network imported!
    echo ========================================
    echo.
    echo The Distribution Group has been saved to the database.
    echo You can now view it in the Visual Editor.
    echo.
) else (
    echo.
    echo ========================================
    echo ERROR: Import failed
    echo ========================================
    echo.
    echo Check the error messages above for details.
    echo.
)

pause
















