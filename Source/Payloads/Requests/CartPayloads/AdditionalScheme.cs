namespace Comanda.WebApi.Payloads;

public sealed record AdditionalScheme
{
    public int AdditionalId { get; init; }
    public int Quantity { get; init; }
}