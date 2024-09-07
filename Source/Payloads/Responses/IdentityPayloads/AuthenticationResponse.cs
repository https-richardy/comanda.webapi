namespace Comanda.WebApi.Payloads;

public record AuthenticationResponse
{
    public string Token { get; init; }
}