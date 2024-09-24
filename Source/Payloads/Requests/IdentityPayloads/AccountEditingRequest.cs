namespace Comanda.WebApi.Payloads;

public sealed record AccountEditingRequest : IRequest<Response>
{
    public string Name { get; init; }
    public string Email { get; init; }
}