## Docker Compose Setup

### Prerequisites
- Docker Desktop installed
- Docker Compose installed

### Quick Start

1. **Build and run all services:**
   ```bash
   docker-compose up --build
   ```

2. **Run in detached mode:**
   ```bash
   docker-compose up -d
   ```

3. **View logs:**
   ```bash
   docker-compose logs -f api
   ```

4. **Stop services:**
   ```bash
   docker-compose down
   ```

5. **Stop and remove volumes:**
   ```bash
   docker-compose down -v
   ```

### Services

- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **SQL Server**: localhost:1433

### Database Credentials

- **Server**: localhost,1433
- **Username**: sa
- **Password**: YourStrong@Passw0rd
- **Database**: AlgoraDb

### Environment Variables

All configuration is in `docker-compose.yml`. Important variables:

- `MSSQL_SA_PASSWORD`: SQL Server password (change in production!)
- `ConnectionStrings__DefaultConnection`: Database connection
- `Jwt__Secret`: JWT signing key (change to your own secret!)

### Migrations

The API automatically runs migrations on startup (when in Development mode).

### Development

To rebuild after code changes:
```bash
docker-compose up --build
```

### Production Deployment

1. Update environment variables in `docker-compose.yml`
2. Change `ASPNETCORE_ENVIRONMENT` to `Production`
3. Use strong passwords and secrets
4. Configure HTTPS certificates
5. Use Docker secrets for sensitive data

### Troubleshooting

**SQL Server won't start:**
- Ensure you have at least 2GB RAM available for Docker
- Check Docker Desktop resource limits

**API can't connect to database:**
- Wait for SQL Server healthcheck to pass (may take 30-60 seconds on first run)
- Check logs: `docker-compose logs sqlserver`

**Port conflicts:**
- Change port mappings in `docker-compose.yml` if 5000 or 1433 are in use
