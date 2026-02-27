using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Infrastructure.Services;

public class ShortCodeGenerator : IShortCodeGenerator
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int CodeLength = 6;
    private static readonly Random Random = new();

    public string Generate()
    {
        return new string(Enumerable
            .Repeat(Chars, CodeLength)
            .Select(s => s[Random.Next(s.Length)])
            .ToArray());
    }
}