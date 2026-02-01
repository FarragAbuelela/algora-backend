using Algora.Application.Common.Interfaces;
using Algora.Domain.Entities;
using Algora.Domain.Enums;
using Algora.Application.Persistence;

namespace Algora.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AlgoraDbContext _context;

    public NotificationService(AlgoraDbContext context)
    {
        _context = context;
    }

    public async Task CreateNotification(Guid userId, string title, string message, NotificationType type, Guid? relatedEntityId = null)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            RelatedEntityId = relatedEntityId
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}

