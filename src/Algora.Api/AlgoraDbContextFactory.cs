using Algora.Application.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Algora.Api;

public class AlgoraDbContextFactory : IDesignTimeDbContextFactory<AlgoraDbContext>
{
    public AlgoraDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AlgoraDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AlgoraDbMulti;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true");
        return new AlgoraDbContext(optionsBuilder.Options);
    }
}

