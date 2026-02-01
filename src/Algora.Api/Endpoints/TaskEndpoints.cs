using Algora.Application.Features.Tasks;
using MediatR;

namespace Algora.Api.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks")
            .WithTags("Tasks")
            .RequireAuthorization();

        group.MapPost("", async (CreateTaskCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("CreateTask");

        group.MapGet("/my-tasks", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetMyTasksQuery())))
            .WithName("GetMyTasks");

        return app;
    }
}
