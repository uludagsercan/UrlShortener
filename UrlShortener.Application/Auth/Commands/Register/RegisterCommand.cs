using MediatR;
using UrlShortener.Domain.Interfaces;

namespace UrlShortener.Application.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password) : IRequest<AuthResult>;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IAuthService _authService;

    public RegisterHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<AuthResult> Handle(RegisterCommand request, CancellationToken ct)
        => _authService.RegisterAsync(request.Email, request.Password);
}