# EPR System Setup Notes

## EmpauerLocal Modules Status

The EmpauerLocal modules are currently commented out in the project due to missing dependencies in the EmpauerLocal codebase. The EPR core functionality works independently.

### To Enable EmpauerLocal Modules

1. Ensure all dependencies in EmpauerLocal are resolved
2. Uncomment the project references in `src/EPR.Web/EPR.Web.csproj`
3. Uncomment the service registrations in `src/EPR.Web/Program.cs`
4. Rebuild the solution

### Current Status

- ✅ Core EPR functionality (Flow Blueprints) - Working
- ✅ Database layer - Configured
- ✅ API endpoints - Implemented
- ✅ Frontend views - Created
- ⚠️ EmpauerLocal modules - Commented out (optional)

## Next Steps

1. **Resolve EmpauerLocal Dependencies**: Fix missing types (UserDto, ThemeModel, Element, ILogger) in EmpauerLocal modules
2. **Add More Features**: Implement material management, product tracking, etc.
3. **Add Authentication**: Once EmpauerLocal modules are working, enable authentication
4. **Add Tests**: Create unit and integration tests
5. **Deploy**: Push to GitHub and deploy via Railway

## Running the Application

```bash
cd src/EPR.Web
dotnet run
```

The application will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger UI: https://localhost:5001/swagger

## Database

The SQLite database (`epr.db`) is created automatically on first run. Default blueprints are seeded automatically.

## API Testing

Use Swagger UI at `/swagger` or test endpoints directly:

```bash
# Get all blueprints
curl https://localhost:5001/api/flowblueprints

# Seed default blueprints
curl -X POST https://localhost:5001/api/flowblueprints/seed-defaults
```









