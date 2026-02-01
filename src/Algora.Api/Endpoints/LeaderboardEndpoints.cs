using Algora.Application.Features.Leaderboard;
using MediatR;

namespace Algora.Api.Endpoints;

public static class LeaderboardEndpoints
{
    public static IEndpointRouteBuilder MapLeaderboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/leaderboard")
            .WithTags("Leaderboard")
            .RequireAuthorization();

        group.MapGet("/camp/{campId}", async (Guid campId, Algora.Domain.Enums.Role? role, int top, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetCampLeaderboardQuery(campId, role, top))))
            .WithName("GetCampLeaderboard");

        return app;
    }
}
