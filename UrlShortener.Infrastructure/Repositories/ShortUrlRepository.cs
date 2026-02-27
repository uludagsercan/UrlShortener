using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Persistence;

namespace UrlShortener.Infrastructure.Repositories;

public class ShortUrlRepository : IShortUrlRepository
{
    private readonly AppDbContext _context;

    public ShortUrlRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode, CancellationToken ct = default)
        => await _context.ShortUrls
            .Include(x => x.Clicks)
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode, ct);

    public async Task<ShortUrl?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.ShortUrls
            .Include(x => x.Clicks)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<ShortUrl>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _context.ShortUrls
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<bool> ShortCodeExistsAsync(string shortCode, CancellationToken ct = default)
        => await _context.ShortUrls.AnyAsync(x => x.ShortCode == shortCode, ct);

    public async Task AddAsync(ShortUrl shortUrl, CancellationToken ct = default)
        => await _context.ShortUrls.AddAsync(shortUrl, ct);

    public async Task AddClickAsync(UrlClick click, CancellationToken ct = default)
        => await _context.UrlClicks.AddAsync(click, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}