namespace Comanda.WebApi.Payloads;

public sealed record OrderItemAdditionalFormatted
{
    public string Name { get; init; }
    public int Quantity { get; init; }

    public static implicit operator OrderItemAdditionalFormatted(OrderItemAdditional item)
    {
        return new OrderItemAdditionalFormatted
        {
            Name = item.Additional.Name,
            Quantity = item.Quantity
        };
    }
}
