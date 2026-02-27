using MediatR;

namespace UrlShortener.Application.Urls.Queries.GetUserUrls;

public record GetUserUrlsQuery(Guid UserId) : IRequest<List<UrlDto>>;

public record UrlDto(
    Guid Id,
    string ShortCode,
    string ShortUrl,
    string OriginalUrl,
    int ClickCount,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);