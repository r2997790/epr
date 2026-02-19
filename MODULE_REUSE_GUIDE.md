# Module Reuse Guide - Using EmpauerLocal Modules in Separate Projects

## Overview

This guide explains how to reuse modules from EmpauerLocal in a separate project/workspace. There are several approaches, each with different trade-offs.

## Available Shared Modules

### Shared Projects (Reusable Components)
- **EmpauerLocal.Shared.UI** - UI components, browser tabs, themes
- **EmpauerLocal.Shared.Auth** - Authentication & authorization
- **EmpauerLocal.Shared.Charts** - Chart management and initialization
- **EmpauerLocal.Shared.Localization** - Internationalization
- **EmpauerLocal.Shared.Common** - Common utilities

### Core Infrastructure
- **EmpauerLocal.Core** - Module orchestrator and IModule interface

### Feature Modules (Optional)
- **EmpauerLocal.Modules.AssessmentNavigator** - Assessment navigation features
- **EmpauerLocal.Modules.AssessmentSearch** - Assessment search
- **EmpauerLocal.Modules.MaterialSearch** - Material search
- **EmpauerLocal.Modules.DIRAdminTools** - Admin tools
- **EmpauerLocal.Modules.SysOp** - System operations

## Approach 1: Git Submodules (Recommended for Development)

**Best for**: Active development, keeping modules in sync, team collaboration

### Setup Steps

1. **Create your new project workspace**
   ```bash
   mkdir MyNewProject
   cd MyNewProject
   git init
   ```

2. **Add EmpauerLocal as a submodule**
   ```bash
   # Add the entire repo as submodule
   git submodule add https://github.com/r2997790/empauer-local.git EmpauerLocal
   
   # Or add specific folders only (more complex but cleaner)
   git submodule add -b main --depth 1 https://github.com/r2997790/empauer-local.git EmpauerLocal
   ```

3. **Create your new solution**
   ```bash
   dotnet new sln -n MyNewProject
   ```

4. **Add your new project**
   ```bash
   dotnet new web -n MyNewProject.Web
   dotnet sln add MyNewProject.Web/MyNewProject.Web.csproj
   ```

5. **Reference shared modules**
   ```xml
   <!-- In MyNewProject.Web.csproj -->
   <ItemGroup>
     <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Shared.UI\EmpauerLocal.Shared.UI.csproj" />
     <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Shared.Auth\EmpauerLocal.Shared.Auth.csproj" />
     <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Shared.Charts\EmpauerLocal.Shared.Charts.csproj" />
     <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Shared.Localization\EmpauerLocal.Shared.Localization.csproj" />
     <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Shared.Common\EmpauerLocal.Shared.Common.csproj" />
     <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Core\EmpauerLocal.Core.csproj" />
   </ItemGroup>
   ```

6. **Update Program.cs**
   ```csharp
   using EmpauerLocal.Core;
   using EmpauerLocal.Shared.UI.BrowserTabs;
   using EmpauerLocal.Shared.Auth.Authentication;
   // ... other using statements
   
   var builder = WebApplication.CreateBuilder(args);
   
   // Register shared services
   builder.Services.AddBrowserTabs();
   builder.Services.AddAuthentication();
   // ... other service registrations
   
   var app = builder.Build();
   
   // Configure shared middleware
   app.UseBrowserTabs();
   // ... other middleware
   
   app.Run();
   ```

### Updating Submodules

```bash
# Update to latest version
git submodule update --remote EmpauerLocal

# Or update all submodules
git submodule update --remote
```

### Pros
- ✅ Keeps modules in sync with source
- ✅ Easy to update
- ✅ Version control friendly
- ✅ Good for active development

### Cons
- ⚠️ Requires Git submodule knowledge
- ⚠️ Slightly more complex setup
- ⚠️ Need to commit submodule reference

---

## Approach 2: NuGet Packages (Recommended for Production)

**Best for**: Versioned releases, production deployments, multiple projects

### Step 1: Create NuGet Packages

Create a script to build and pack shared projects:

**`build-packages.ps1`**:
```powershell
# Build and pack shared projects
$projects = @(
    "src\EmpauerLocal.Core\EmpauerLocal.Core.csproj",
    "src\EmpauerLocal.Shared.UI\EmpauerLocal.Shared.UI.csproj",
    "src\EmpauerLocal.Shared.Auth\EmpauerLocal.Shared.Auth.csproj",
    "src\EmpauerLocal.Shared.Charts\EmpauerLocal.Shared.Charts.csproj",
    "src\EmpauerLocal.Shared.Localization\EmpauerLocal.Shared.Localization.csproj",
    "src\EmpauerLocal.Shared.Common\EmpauerLocal.Shared.Common.csproj"
)

foreach ($project in $projects) {
    dotnet pack $project -c Release -o ./packages
}
```

### Step 2: Create Local NuGet Feed

**Option A: Local Folder Feed**
```bash
# Create packages folder
mkdir packages

# Build packages
dotnet pack src/EmpauerLocal.Core/EmpauerLocal.Core.csproj -c Release -o ./packages
dotnet pack src/EmpauerLocal.Shared.UI/EmpauerLocal.Shared.UI.csproj -c Release -o ./packages
# ... repeat for other projects

# Add to NuGet.config in your new project
```

**Option B: NuGet Server (for team)**
```bash
# Use BaGet or NuGet.Server
docker run -d -p 5000:80 --name nuget-server loicsharma/baget
```

### Step 3: Configure NuGet Feed

**`NuGet.config` in your new project**:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Local" value="C:\path\to\EmpauerLocal\packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

### Step 4: Install Packages

```bash
cd MyNewProject
dotnet add package EmpauerLocal.Core --version 1.0.0 --source C:\path\to\EmpauerLocal\packages
dotnet add package EmpauerLocal.Shared.UI --version 1.0.0 --source C:\path\to\EmpauerLocal\packages
dotnet add package EmpauerLocal.Shared.Auth --version 1.0.0 --source C:\path\to\EmpauerLocal\packages
# ... repeat for other packages
```

### Pros
- ✅ Version control
- ✅ Clean separation
- ✅ Easy to distribute
- ✅ Production-ready

### Cons
- ⚠️ Requires package versioning
- ⚠️ More setup initially
- ⚠️ Need to rebuild packages for updates

---

## Approach 3: Direct Project References (Simplest)

**Best for**: Quick prototyping, single developer, same machine

### Setup Steps

1. **Clone EmpauerLocal repository**
   ```bash
   git clone https://github.com/r2997790/empauer-local.git
   ```

2. **Create your new project in a sibling folder**
   ```
   workspace/
   ├── empauer-local/
   └── my-new-project/
   ```

3. **Add project references**
   ```xml
   <!-- In MyNewProject.Web.csproj -->
   <ItemGroup>
     <ProjectReference Include="..\..\empauer-local\src\EmpauerLocal.Shared.UI\EmpauerLocal.Shared.UI.csproj" />
     <ProjectReference Include="..\..\empauer-local\src\EmpauerLocal.Shared.Auth\EmpauerLocal.Shared.Auth.csproj" />
     <!-- ... other references -->
   </ItemGroup>
   ```

### Pros
- ✅ Simplest setup
- ✅ Immediate access to source code
- ✅ Easy debugging

### Cons
- ⚠️ Tight coupling
- ⚠️ Hard to version
- ⚠️ Not ideal for teams

---

## Approach 4: Git Subtree (Alternative to Submodules)

**Best for**: Want code in your repo but keep it synced

```bash
# Add subtree
git subtree add --prefix=EmpauerLocal --squash https://github.com/r2997790/empauer-local.git main

# Update subtree
git subtree pull --prefix=EmpauerLocal --squash https://github.com/r2997790/empauer-local.git main
```

### Pros
- ✅ Code lives in your repo
- ✅ No submodule complexity
- ✅ Can modify locally

### Cons
- ⚠️ Larger repo size
- ⚠️ Merge conflicts possible
- ⚠️ Less clean separation

---

## Recommended Approach: Git Submodules

For most use cases, **Git Submodules** is the best choice because:
1. Keeps modules separate but synced
2. Easy to update
3. Version control friendly
4. Works well for teams

## Quick Start with Submodules

```bash
# 1. Create new project
mkdir MyNewProject && cd MyNewProject
git init
dotnet new sln -n MyNewProject
dotnet new web -n MyNewProject.Web
dotnet sln add MyNewProject.Web/MyNewProject.Web.csproj

# 2. Add EmpauerLocal as submodule
git submodule add https://github.com/r2997790/empauer-local.git EmpauerLocal

# 3. Add project references (edit MyNewProject.Web.csproj)
# See Approach 1 above

# 4. Restore and build
dotnet restore
dotnet build
```

## Updating Modules

```bash
# Update to latest
cd EmpauerLocal
git pull origin main
cd ..
git add EmpauerLocal
git commit -m "Update EmpauerLocal modules"
```

## Module Dependencies

When using modules, be aware of dependencies:

- **EmpauerLocal.Shared.UI** → Requires **EmpauerLocal.Core**
- **EmpauerLocal.Shared.Auth** → Requires **EmpauerLocal.Core**, **EmpauerLocal.Data**
- **EmpauerLocal.Shared.Charts** → Requires **EmpauerLocal.Core**
- **EmpauerLocal.Shared.Localization** → Requires **EmpauerLocal.Core**

## Example: Minimal Setup

**MyNewProject.Web.csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- Core infrastructure -->
    <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Core\EmpauerLocal.Core.csproj" />
    
    <!-- Shared modules -->
    <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Shared.UI\EmpauerLocal.Shared.UI.csproj" />
    <ProjectReference Include="..\EmpauerLocal\src\EmpauerLocal.Shared.Auth\EmpauerLocal.Shared.Auth.csproj" />
  </ItemGroup>
</Project>
```

**Program.cs**:
```csharp
using EmpauerLocal.Core;
using EmpauerLocal.Shared.UI.BrowserTabs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Register shared services
builder.Services.AddBrowserTabs();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseBrowserTabs(); // Add browser tabs support

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

## Troubleshooting

### Issue: "Project not found"
- Ensure submodule is initialized: `git submodule update --init --recursive`
- Check path references are correct

### Issue: "Dependency conflicts"
- Ensure all projects target same .NET version (net8.0)
- Check NuGet package versions match

### Issue: "Build errors"
- Run `dotnet restore` first
- Check all dependencies are referenced

## Next Steps

1. Choose your approach (recommend Git Submodules)
2. Set up your new project
3. Add module references
4. Configure services in Program.cs
5. Start using shared components!

