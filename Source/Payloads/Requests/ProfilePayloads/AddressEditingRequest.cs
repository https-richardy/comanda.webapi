namespace Comanda.WebApi.Payloads;

public sealed record AddressEditingRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int AddressId { get; set; }

    public string PostalCode { get; init; }
    public string? Number { get; init; }
    public string? Complement { get; init; }
    public string? Reference { get; init; }
}