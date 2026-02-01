using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Application.Persistence;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Algora.Application.Features.Notifications;

public record GetMyNotificationsQuery(bool UnreadOnly) : IRequest<NotificationsListResponse>;

public record NotificationsListResponse(List<NotificationDto> Notifications);
public record NotificationDto(Guid Id, string Title, string Message, bool IsRead, DateTime CreatedAt);

public class GetMyNotificationsHandler : IRequestHandler<GetMyNotificationsQuery, NotificationsListResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyNotificationsHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<NotificationsListResponse> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var query = _context.Notifications.Where(n => n.UserId == currentUserId);

        if (request.UnreadOnly)
            query = query.Where(n => !n.IsRead);

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .Select(n => new NotificationDto(n.Id, n.Title, n.Message, n.IsRead, n.CreatedAt))
            .ToListAsync(cancellationToken);

        return new NotificationsListResponse(notifications);
    }
}







