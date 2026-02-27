using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<ShortUrl> ShortUrls { get; set; } = new List<ShortUrl>();
}