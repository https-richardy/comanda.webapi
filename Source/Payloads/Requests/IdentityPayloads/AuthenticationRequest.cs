namespace Comanda.WebApi.Payloads;

public sealed record AuthenticationCredentials : IRequest<Response<AuthenticationResponse>>
{
    public string Email { get; init; }
    public string Password { get; init; }
}