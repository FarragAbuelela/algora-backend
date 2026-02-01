using Algora.Application.Features.Mentorship;
using MediatR;

namespace Algora.Api.Endpoints;

public static class MentorshipEndpoints
{
    public static IEndpointRouteBuilder MapMentorshipEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/mentorship")
            .WithTags("Mentorship")
            .RequireAuthorization();

        group.MapPost("/assign", async (AssignStudentToMentorCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("AssignStudentToMentor");

        return app;
    }
}
