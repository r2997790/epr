@echo off
setlocal

echo ========================================
echo Seed Admin User
echo ========================================
echo.

:: Check if we're in the right directory
if not exist "src\EPR.Web\EPR.Web.csproj" (
    echo ERROR: EPR.Web project not found.
    echo Please ensure you're running this from the EPR root directory.
    echo.
    pause
    exit /b 1
)

echo Running seed admin user script...
echo.

:: Run the seed command
dotnet run --project src\EPR.Web -- seed-admin-user

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo SUCCESS: Admin user seeded!
    echo ========================================
    echo.
    echo Default credentials:
    echo   Username: admin
    echo   Password: admin123
    echo.
) else (
    echo.
    echo ========================================
    echo ERROR: Seeding failed
    echo ========================================
    echo.
    echo Check the error messages above for details.
    echo.
)

pause
















