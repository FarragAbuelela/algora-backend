using Algora.Application.Features.Sessions;
using MediatR;

namespace Algora.Api.Endpoints;

public static class SessionEndpoints
{
    public static IEndpointRouteBuilder MapSessionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sessions")
            .WithTags("Sessions")
            .RequireAuthorization();

        group.MapPost("", async (CreateSessionCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("CreateSession");

        group.MapGet("/my-sessions", async (DateTime? from, DateTime? to, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetMySessionsQuery(from, to))))
            .WithName("GetMySessions");

        return app;
    }
}
