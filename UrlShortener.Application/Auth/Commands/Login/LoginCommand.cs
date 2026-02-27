using MediatR;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResult>
{
    private readonly IAuthService _authService;

    public LoginHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<AuthResult> Handle(LoginCommand request, CancellationToken ct)
        => _authService.LoginAsync(request.Email, request.Password);
}