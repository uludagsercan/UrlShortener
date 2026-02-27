namespace UrlShortener.Domain.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password);
    Task<AuthResult> LoginAsync(string email, string password);
}

public record AuthResult(
    bool Success,
    string? Token,
    string? Email,
    string? Error
);