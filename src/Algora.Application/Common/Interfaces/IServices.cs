using Algora.Domain.Entities;
using Algora.Domain.Enums;
using System.Security.Claims;

namespace Algora.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, IEnumerable<UserRole> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

public interface INotificationService
{
    Task CreateNotification(Guid userId, string title, string message, NotificationType type, Guid? relatedEntityId = null);
}

public interface IAlgoraDbContext
{
    // Not needed - will inject DbContext directly from Infrastructure
}



