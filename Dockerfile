FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/EPR.Web/EPR.Web.csproj", "src/EPR.Web/"]
COPY ["src/EPR.Application/EPR.Application.csproj", "src/EPR.Application/"]
COPY ["src/EPR.Data/EPR.Data.csproj", "src/EPR.Data/"]
COPY ["src/EPR.Domain/EPR.Domain.csproj", "src/EPR.Domain/"]
RUN dotnet restore "src/EPR.Web/EPR.Web.csproj"
COPY . .
WORKDIR "/src/src/EPR.Web"
RUN dotnet build "EPR.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EPR.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EPR.Web.dll"]









