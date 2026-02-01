using Algora.Application.Features.Auth;
using MediatR;

namespace Algora.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", async (RegisterCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("Register");

        group.MapPost("/login", async (LoginCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("Login");

        group.MapPost("/refresh", async (RefreshTokenCommand command, IMediator mediator) =>
            Results.Ok(await mediator.Send(command)))
            .WithName("RefreshToken");

        return app;
    }
}
