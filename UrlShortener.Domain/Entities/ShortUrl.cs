namespace UrlShortener.Domain.Entities;

public class ShortUrl
{
    public Guid Id { get; private set; }
    public string OriginalUrl { get; private set; }
    public string ShortCode { get; private set; }
    public Guid? UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<UrlClick> Clicks { get; private set; } = new List<UrlClick>();

    private ShortUrl() { }

    public static ShortUrl Create(string originalUrl, string shortCode, Guid? userId = null, DateTime? expiresAt = null)
    {
        return new ShortUrl
        {
            Id = Guid.NewGuid(),
            OriginalUrl = originalUrl,
            ShortCode = shortCode,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            IsActive = true
        };
    }

    public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    
    public void Deactivate() => IsActive = false;
}