using Algora.Application.Features.Notifications;
using MediatR;

namespace Algora.Api.Endpoints;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications")
            .WithTags("Notifications")
            .RequireAuthorization();

        group.MapGet("", async (bool unreadOnly, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetMyNotificationsQuery(unreadOnly))))
            .WithName("GetMyNotifications");

        return app;
    }
}
