namespace Comanda.WebApi.Payloads;

public sealed record FormattedOrder
{
    public int Id { get; init; }
    public string Customer { get; init; }
    public string ShippingAddress { get; init; }
    public decimal Total { get; init; }
    public EOrderStatus Status { get; init; }
    public DateTime Date { get; init; }

    public FormattedOrder()
    {

    }

    public FormattedOrder(
        int id,
        string customer,
        string shippingAddress,
        decimal total,
        EOrderStatus status,
        DateTime date
    )
    {
        Id = id;
        Customer = customer;
        ShippingAddress = shippingAddress;
        Total = total;
        Status = status;
        Date = date;
    }
}