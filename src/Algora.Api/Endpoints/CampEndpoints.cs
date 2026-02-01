using Algora.Application.Features.Camps;
using MediatR;

namespace Algora.Api.Endpoints;

public static class CampEndpoints
{
    public static IEndpointRouteBuilder MapCampEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/camps")
            .WithTags("Camps")
            .RequireAuthorization();

        group.MapPost("", async (CreateCampCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("CreateCamp");

        group.MapPost("/{campId}/sheets", async (Guid campId, AddCampSheetCommand command, IMediator mediator) =>
            Results.Created($"/api/camps/{campId}/sheets", await mediator.Send(command with { CampId = campId })))
            .WithName("AddCampSheet");

        group.MapPost("/{campId}/problems", async (Guid campId, AddProblemCommand command, IMediator mediator) =>
            Results.Created($"/api/camps/{campId}/problems", await mediator.Send(command with { CampId = campId })))
            .WithName("AddProblem");

        return app;
    }
}
