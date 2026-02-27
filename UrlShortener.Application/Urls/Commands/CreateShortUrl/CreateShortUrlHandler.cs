using MediatR;
using Microsoft.Extensions.Options;
using UrlShortener.Application.Common.Settings;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Urls.Commands.CreateShortUrl;

public class CreateShortUrlHandler : IRequestHandler<CreateShortUrlCommand, CreateShortUrlResult>
{
    private readonly IShortUrlRepository _repository;
    private readonly IShortCodeGenerator _generator;
    private readonly AppSettings _settings;

    public CreateShortUrlHandler(
        IShortUrlRepository repository,
        IShortCodeGenerator generator,
        IOptions<AppSettings> settings)
    {
        _repository = repository;
        _generator = generator;
        _settings = settings.Value;
    }

    public async Task<CreateShortUrlResult> Handle(CreateShortUrlCommand request, CancellationToken ct)
    {
        var shortCode = request.CustomAlias ?? await GenerateUniqueCodeAsync(ct);

        if (await _repository.ShortCodeExistsAsync(shortCode, ct))
            throw new InvalidOperationException($"'{shortCode}' alias zaten kullanımda.");

        var shortUrl = ShortUrl.Create(
            request.OriginalUrl,
            shortCode,
            request.UserId,
            request.ExpiresAt
        );

        await _repository.AddAsync(shortUrl, ct);
        await _repository.SaveChangesAsync(ct);

        return new CreateShortUrlResult(
            shortUrl.Id,
            shortUrl.ShortCode,
            $"{_settings.BaseUrl}/{shortUrl.ShortCode}",  // ← IConfiguration yerine
            shortUrl.OriginalUrl,
            shortUrl.CreatedAt
        );
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken ct)
    {
        string code;
        do
        {
            code = _generator.Generate();
        } while (await _repository.ShortCodeExistsAsync(code, ct));

        return code;
    }
}