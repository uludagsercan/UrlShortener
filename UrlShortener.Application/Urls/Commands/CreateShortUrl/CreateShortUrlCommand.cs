using MediatR;

namespace UrlShortener.Application.Urls.Commands.CreateShortUrl;

public record CreateShortUrlCommand(
    string OriginalUrl,
    string? CustomAlias,
    DateTime? ExpiresAt,
    Guid? UserId
) : IRequest<CreateShortUrlResult>;

public record CreateShortUrlResult(
    Guid Id,
    string ShortCode,
    string ShortUrl,
    string OriginalUrl,
    DateTime CreatedAt
);