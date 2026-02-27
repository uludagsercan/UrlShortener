using MediatR;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Urls.Queries.ResolveShortUrl;

public class ResolveShortUrlHandler : IRequestHandler<ResolveShortUrlQuery, string>
{
    private readonly IShortUrlRepository _repository;
    private readonly ICacheService _cache;

    public ResolveShortUrlHandler(IShortUrlRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<string> Handle(ResolveShortUrlQuery request, CancellationToken ct)
    {
        // 1. Önce Redis'e bak
        var cached = await _cache.GetAsync($"url:{request.ShortCode}");
        if (cached != null)
        {
            await RecordClickAsync(request.ShortCode, request.IpAddress, request.UserAgent, ct);
            return cached;
        }

        // 2. DB'ye git
        var shortUrl = await _repository.GetByShortCodeAsync(request.ShortCode, ct)
            ?? throw new KeyNotFoundException($"'{request.ShortCode}' bulunamadı.");

        if (!shortUrl.IsActive || shortUrl.IsExpired())
            throw new InvalidOperationException("Bu link artık aktif değil.");

        // 3. Redis'e yaz
        await _cache.SetAsync($"url:{request.ShortCode}", shortUrl.OriginalUrl, TimeSpan.FromHours(1));

        // 4. Click kaydet — artık await ile
        await RecordClickAsync(request.ShortCode, request.IpAddress, request.UserAgent, ct);

        return shortUrl.OriginalUrl;
    }

    private async Task RecordClickAsync(string shortCode, string? ip, string? ua, CancellationToken ct)
    {
        var shortUrl = await _repository.GetByShortCodeAsync(shortCode, ct);
        if (shortUrl is null) return;

        var click = UrlClick.Create(shortUrl.Id, ip, ua);
        await _repository.AddClickAsync(click, ct);
        await _repository.SaveChangesAsync(ct);
    }
}