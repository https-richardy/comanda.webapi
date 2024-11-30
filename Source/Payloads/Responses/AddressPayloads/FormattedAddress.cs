namespace Comanda.WebApi.Payloads;

public sealed record FormattedAddress
{
    public int Id { get; init; }
    public string Street { get; init; } = null!;
    public string Number { get; init; } = null!;
    public string City { get; init; } = null!;
    public string State { get; init; } = null!;
    public string Neighborhood { get; init; } = null!;
    public string PostalCode { get; init; } = null!;
    public string? Complement { get; init; } = null!;
    public string? Reference { get; init; } = null!;
}