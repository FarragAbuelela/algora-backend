# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Algora.sln", "./"]
COPY ["src/Algora.Api/Algora.Api.csproj", "src/Algora.Api/"]
COPY ["src/Algora.Application/Algora.Application.csproj", "src/Algora.Application/"]
COPY ["src/Algora.Domain/Algora.Domain.csproj", "src/Algora.Domain/"]
COPY ["src/Algora.Infrastructure/Algora.Infrastructure.csproj", "src/Algora.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Algora.sln"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/Algora.Api"
RUN dotnet build "Algora.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Algora.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Algora.Api.dll"]