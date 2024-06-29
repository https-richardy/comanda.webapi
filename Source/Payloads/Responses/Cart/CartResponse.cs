namespace Comanda.WebApi.Payloads;

public sealed record CartResponse
{
    public decimal Total { get; init; }
    public ICollection<FormattedCartItem> Items { get; init; } = [];
}