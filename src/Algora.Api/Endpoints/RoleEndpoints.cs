using Algora.Application.Features.Roles;
using MediatR;

namespace Algora.Api.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles")
            .WithTags("Roles")
            .RequireAuthorization();

        group.MapPost("/upgrade-community-leader", async (UpgradeToCommunityLeaderCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("UpgradeToCommunityLeader");

        return app;
    }
}
