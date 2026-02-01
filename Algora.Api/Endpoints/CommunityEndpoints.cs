using Algora.Application.Features.Communities;
using MediatR;

namespace Algora.Api.Endpoints;

public static class CommunityEndpoints
{
    public static IEndpointRouteBuilder MapCommunityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/communities")
            .WithTags("Communities")
            .RequireAuthorization();

        group.MapPost("", async (CreateCommunityCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("CreateCommunity");

        group.MapGet("", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListCommunitiesQuery())))
            .WithName("ListCommunities");

        return app;
    }
}
