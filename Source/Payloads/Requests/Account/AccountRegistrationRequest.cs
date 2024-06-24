namespace Comanda.WebApi.Payloads;

public sealed record AccountRegistrationRequest : IRequest<Response>
{
    public string Email { get; init; }
    public string Password { get; init; }
    public EAccountType AccountType { get; init; }
}