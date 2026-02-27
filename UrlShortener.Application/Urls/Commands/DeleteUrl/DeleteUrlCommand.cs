using MediatR;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Urls.Commands.DeleteUrl;

public record DeleteUrlCommand(Guid Id, Guid UserId) : IRequest;

public class DeleteUrlHandler : IRequestHandler<DeleteUrlCommand>
{
    private readonly IShortUrlRepository _repository;
    private readonly ICacheService _cache;

    public DeleteUrlHandler(IShortUrlRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task Handle(DeleteUrlCommand request, CancellationToken ct)
    {
        var url = await _repository.GetByIdAsync(request.Id, ct)
            ?? throw new KeyNotFoundException("Link bulunamadı.");

        if (url.UserId != request.UserId)
            throw new UnauthorizedAccessException("Bu linki silme yetkiniz yok.");

        url.Deactivate();
        await _cache.RemoveAsync($"url:{url.ShortCode}");
        await _repository.SaveChangesAsync(ct);
    }
}