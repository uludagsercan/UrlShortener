using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Interfaces;

public interface IShortUrlRepository
{
    Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken ct = default);
    Task<ShortUrl?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<ShortUrl>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken ct = default);
    Task AddAsync(ShortUrl shortUrl, CancellationToken ct = default);
    Task AddClickAsync(UrlClick click, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}