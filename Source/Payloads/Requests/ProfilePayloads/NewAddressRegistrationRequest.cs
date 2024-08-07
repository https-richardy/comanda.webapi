namespace Comanda.WebApi.Payloads;

public sealed record NewAddressRegistrationRequest : IRequest<Response>
{
    public string PostalCode { get; init; }
    public string? Number { get; init; }
    public string? Complement { get; init; }
    public string? Reference { get; init; }
}