using Algora.Application.Features.Users;
using MediatR;

namespace Algora.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapPost("/{userId}/roles", async (Guid userId, AssignRoleCommand command, IMediator mediator) =>
            Results.Created($"/api/users/{userId}/roles", await mediator.Send(command with { UserId = userId })))
            .WithName("AssignRole");

        group.MapGet("/profile", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetUserProfileQuery())))
            .WithName("GetMyProfile");

        return app;
    }
}
