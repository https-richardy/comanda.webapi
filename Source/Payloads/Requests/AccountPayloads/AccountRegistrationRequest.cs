namespace Comanda.WebApi.Payloads;

public sealed record AccountRegistrationRequest : IRequest<Response>
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}