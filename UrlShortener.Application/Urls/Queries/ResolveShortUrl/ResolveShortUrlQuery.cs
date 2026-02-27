using MediatR;

namespace UrlShortener.Application.Urls.Queries.ResolveShortUrl;

public record ResolveShortUrlQuery(
    string ShortCode,
    string? IpAddress,
    string? UserAgent
) : IRequest<string>; 