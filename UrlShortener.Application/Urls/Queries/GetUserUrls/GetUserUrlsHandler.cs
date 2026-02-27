using MediatR;
using Microsoft.Extensions.Options;
using UrlShortener.Application.Common.Settings;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Urls.Queries.GetUserUrls;

public class GetUserUrlsHandler : IRequestHandler<GetUserUrlsQuery, List<UrlDto>>
{
    private readonly IShortUrlRepository _repository;
    private readonly AppSettings _settings;

    public GetUserUrlsHandler(IShortUrlRepository repository, IOptions<AppSettings> settings)
    {
        _repository = repository;
        _settings = settings.Value;
    }

    public async Task<List<UrlDto>> Handle(GetUserUrlsQuery request, CancellationToken ct)
    {
        var urls = await _repository.GetByUserIdAsync(request.UserId, ct);

        return urls.Select(u => new UrlDto(
            u.Id,
            u.ShortCode,
            $"{_settings.BaseUrl}/{u.ShortCode}",
            u.OriginalUrl,
            u.Clicks.Count,
            u.CreatedAt,
            u.ExpiresAt
        )).ToList();
    }
}