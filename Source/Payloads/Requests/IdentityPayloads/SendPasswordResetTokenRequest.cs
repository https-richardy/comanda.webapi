namespace Comanda.WebApi.Payloads;

public sealed record SendPasswordResetTokenRequest : IRequest<Response>
{
    public string Email { get; init; }
}