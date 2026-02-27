namespace UrlShortener.Domain.Entities;

public class UrlClick
{
    public Guid Id { get; private set; }
    public Guid ShortUrlId { get; private set; }
    public ShortUrl ShortUrl { get; private set; }
    public DateTime ClickedAt { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }

    private UrlClick() { }

    public static UrlClick Create(Guid shortUrlId, string? ipAddress, string? userAgent)
    {
        return new UrlClick
        {
            Id = Guid.NewGuid(),
            ShortUrlId = shortUrlId,
            ClickedAt = DateTime.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
    }
}