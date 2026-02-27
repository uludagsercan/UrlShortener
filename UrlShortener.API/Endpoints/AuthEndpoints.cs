using MediatR;
using UrlShortener.Application.Auth.Commands.Login;
using UrlShortener.Application.Auth.Commands.Register;

namespace UrlShortener.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.Success
                ? Results.Ok(result)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/login", async (LoginCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.Success
                ? Results.Ok(result)
                : Results.Unauthorized();
        });
    }
}