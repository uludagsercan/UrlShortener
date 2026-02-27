using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Urls.Commands.CreateShortUrl;
using UrlShortener.Application.Urls.Commands.DeleteUrl;
using UrlShortener.Application.Urls.Queries.GetUserUrls;
using UrlShortener.Application.Urls.Queries.ResolveShortUrl;

namespace UrlShortener.API.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this IEndpointRouteBuilder app)
{
    // 1. Önce spesifik route'lar
    app.MapPost("/api/urls", async (
        CreateShortUrlCommand command,
        IMediator mediator,
        HttpContext ctx) =>
    {
        var userIdClaim = ctx.User.FindFirst("sub")
                       ?? ctx.User.FindFirst(ClaimTypes.NameIdentifier);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        var commandWithUser = command with { UserId = userId };
        var result = await mediator.Send(commandWithUser);
        return Results.Created($"/api/urls/{result.Id}", result);
    }).RequireAuthorization();

    app.MapPost("/api/urls/anonymous", async (
        CreateShortUrlCommand command,
        IMediator mediator) =>
    {
        var commandWithoutUser = command with { UserId = null };
        var result = await mediator.Send(commandWithoutUser);
        return Results.Created($"/api/urls/{result.Id}", result);
    });

    app.MapGet("/api/urls", async (
        IMediator mediator,
        HttpContext ctx) =>
    {
        var userIdClaim = ctx.User.FindFirst("sub")
                       ?? ctx.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Results.Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);
        var result = await mediator.Send(new GetUserUrlsQuery(userId));
        return Results.Ok(result);
    }).RequireAuthorization();

    app.MapDelete("/api/urls/{id}", async (
        Guid id,
        IMediator mediator,
        HttpContext ctx) =>
    {
        var userIdClaim = ctx.User.FindFirst("sub")
                       ?? ctx.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Results.Unauthorized();
        var userId = Guid.Parse(userIdClaim.Value);
        await mediator.Send(new DeleteUrlCommand(id, userId));
        return Results.NoContent();
    }).RequireAuthorization();

    // 2. En sona generic shortCode route
    app.MapGet("/{shortCode}", async (
        string shortCode,
        HttpContext ctx,
        IMediator mediator) =>
    {
        var ignoredPaths = new[] { "favicon.ico", "apple-touch-icon.png", "apple-touch-icon-precomposed.png", "robots.txt" };
        if (ignoredPaths.Contains(shortCode.ToLower()))
            return Results.NotFound();

        try
        {
            var ip = ctx.Connection.RemoteIpAddress?.ToString();
            var ua = ctx.Request.Headers.UserAgent.ToString();
            var originalUrl = await mediator.Send(new ResolveShortUrlQuery(shortCode, ip, ua));
            return Results.Redirect(originalUrl, permanent: false);
        }
        catch (KeyNotFoundException)
        {
            return Results.NotFound($"'{shortCode}' bulunamadı.");
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    });
}
}