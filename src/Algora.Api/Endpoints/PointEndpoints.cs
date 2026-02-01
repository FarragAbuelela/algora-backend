using Algora.Application.Features.Points;
using MediatR;

namespace Algora.Api.Endpoints;

public static class PointEndpoints
{
    public static IEndpointRouteBuilder MapPointEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/points")
            .WithTags("Points")
            .RequireAuthorization();

        group.MapPost("", async (AddPointsCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("AddPoints");

        group.MapGet("/{userId}/{campId}", async (Guid userId, Guid campId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetUserPointsQuery(userId, campId))))
            .WithName("GetUserPoints");

        return app;
    }
}
