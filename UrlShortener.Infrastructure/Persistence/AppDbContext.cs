using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();
    public DbSet<UrlClick> UrlClicks => Set<UrlClick>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Identity tabloları için şart

        builder.Entity<ShortUrl>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ShortCode).IsUnique();
            e.Property(x => x.OriginalUrl).HasMaxLength(2048);
            e.Property(x => x.ShortCode).HasMaxLength(20);
        });

        builder.Entity<UrlClick>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.ShortUrl)
                .WithMany(x => x.Clicks)
                .HasForeignKey(x => x.ShortUrlId);
        });
    }
}