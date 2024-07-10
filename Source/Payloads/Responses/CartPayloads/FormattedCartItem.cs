namespace Comanda.WebApi.Payloads;

public record FormattedCartItem
{
    public int Id { get; init; }
    public string Title { get; init; }
    public string ImageUrl { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
}