using Algora.Application.Features.Teams;
using MediatR;

namespace Algora.Api.Endpoints;

public static class TeamEndpoints
{
    public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/teams")
            .WithTags("Teams")
            .RequireAuthorization();

        group.MapPost("", async (CreateTeamCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("CreateTeam");

        return app;
    }
}
