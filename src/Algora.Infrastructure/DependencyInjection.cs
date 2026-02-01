using Algora.Infrastructure.Authentication;
using Algora.Application.Persistence;
using Algora.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Algora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AlgoraDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<Application.Common.Interfaces.IJwtService, JwtService>();
        services.AddScoped<Application.Common.Interfaces.INotificationService, NotificationService>();

        return services;
    }
}

