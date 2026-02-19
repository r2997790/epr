@echo off
echo ========================================
echo Distribution ASN - Database Initialization
echo ========================================
echo.
echo This will initialize the database tables for ASN Distribution management.
echo.
cd src\EPR.Web
dotnet run init-asn-db
echo.
echo ========================================
echo Initialization Complete!
echo ========================================
echo.
echo You can now run the application:
echo    cd src\EPR.Web
echo    dotnet run
echo.
echo Then navigate to: http://localhost:5290/Distribution
echo.
pause
