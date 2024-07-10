namespace Comanda.WebApi.Payloads;

public record AuthenticationResponse
{
    public required string Token { get; init; }
}