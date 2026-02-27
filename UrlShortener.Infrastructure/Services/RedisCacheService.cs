using Microsoft.Extensions.Caching.Distributed;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<string?> GetAsync(string key)
        => await _cache.GetStringAsync(key);

    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
            options.SetAbsoluteExpiration(expiry.Value);

        await _cache.SetStringAsync(key, value, options);
    }

    public async Task RemoveAsync(string key)
        => await _cache.RemoveAsync(key);
}