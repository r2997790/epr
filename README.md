# EPR System - Extended Producer Responsibility Management

A world-class EPR (Extended Producer Responsibility) software system for managing packaging materials, products, geographies, jurisdictions, and fee schemes. Built with .NET 8, ASP.NET Core MVC, and Entity Framework Core.

## Features

- **Flow Blueprints**: Manage Sankey flow blueprints for visualizing EPR data flows
- **Material Management**: Track material classifications and packaging types
- **Product Tracking**: Manage products (SKUs) across geographies and jurisdictions
- **Fee Schemes**: Configure and manage fee/tax schemes by jurisdiction
- **Modular Architecture**: Built with clean architecture principles for extensibility
- **EmpauerLocal Integration**: Reuses authentication, UI components, and other modules from EmpauerLocal

## Architecture

The solution follows a clean, modular architecture:

- **EPR.Domain**: Domain entities and business logic
- **EPR.Data**: Data access layer with Entity Framework Core
- **EPR.Application**: Application services and business logic
- **EPR.Web**: ASP.NET Core MVC web application

## Prerequisites

- .NET 8.0 SDK or later
- Git (for submodule management)
- SQLite (for local development)

## Getting Started

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd EPR
```

### 2. Initialize Submodules

```bash
git submodule update --init --recursive
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the Application

```bash
cd src/EPR.Web
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

### 6. Seed Default Blueprints

The default Sankey blueprints are automatically seeded on first startup. You can also manually seed them via the API:

```bash
POST /api/flowblueprints/seed-defaults
```

## API Endpoints

### Flow Blueprints

- `GET /api/flowblueprints` - Get all flow blueprints
- `GET /api/flowblueprints/{key}` - Get a blueprint by key
- `POST /api/flowblueprints/seed-defaults` - Seed default blueprints

### Swagger UI

In development mode, Swagger UI is available at `/swagger`.

## Database

The application uses SQLite by default for local development. The database file (`epr.db`) is created automatically on first run.

### Connection String

Configure the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=epr.db"
  }
}
```

## EmpauerLocal Modules

This project reuses modules from EmpauerLocal via Git submodule:

- **EmpauerLocal.Core**: Module orchestrator
- **EmpauerLocal.Shared.UI**: UI components and browser tabs
- **EmpauerLocal.Shared.Auth**: Authentication and authorization
- **EmpauerLocal.Shared.Charts**: Chart management
- **EmpauerLocal.Shared.Localization**: Internationalization
- **EmpauerLocal.Shared.Common**: Common utilities

### Updating EmpauerLocal Submodule

```bash
cd EmpauerLocal
git pull origin main
cd ..
git add EmpauerLocal
git commit -m "Update EmpauerLocal modules"
```

## Deployment

### Railway Deployment

The project includes Railway configuration files:

- `railway.json` - Railway build and deploy configuration
- `railway.toml` - Alternative Railway configuration
- `Dockerfile` - Docker configuration for containerized deployment

#### Deploying to Railway

1. Connect your GitHub repository to Railway
2. Railway will automatically detect the project and use the Dockerfile
3. Configure environment variables as needed
4. The application will be deployed automatically on push

### Local Docker Deployment

```bash
docker build -t epr-system .
docker run -p 8080:8080 epr-system
```

## Project Structure

```
EPR/
├── src/
│   ├── EPR.Domain/          # Domain entities
│   ├── EPR.Data/            # Data access layer
│   ├── EPR.Application/     # Application services
│   └── EPR.Web/             # Web application
├── EmpauerLocal/            # Git submodule (EmpauerLocal modules)
├── default_sankey_blueprints.json
├── schema_v4.sql
├── openapi_v4.yaml
├── Dockerfile
├── railway.json
└── README.md
```

## Development

### Adding New Features

1. Create domain entities in `EPR.Domain/Entities`
2. Add DbSet to `EPRDbContext` in `EPR.Data`
3. Create application services in `EPR.Application/Services`
4. Add controllers in `EPR.Web/Controllers`
5. Create views in `EPR.Web/Views`

### Running Tests

```bash
dotnet test
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

[Specify your license here]

## Support

For issues and questions, please open an issue on GitHub.









