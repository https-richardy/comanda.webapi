namespace Comanda.WebApi.Payloads;

public sealed record FormattedOrderDetails
{
    public int Id { get; init; }
    public string Customer { get; init; }
    public string ShippingAddress { get; init; }
    public decimal Total { get; init; }

    public ICollection<OrderItemFormatted> Items { get; init; } = [];

    public EOrderStatus Status { get; init; }
    public DateTime Date { get; init; }

    public static implicit operator FormattedOrderDetails(Order order)
    {
        var formattedOrderDetails = new FormattedOrderDetails
        {
            Id = order.Id,
            Customer = order.Customer.FullName ?? string.Empty,
            ShippingAddress = AddressFormatter.FormatComplete(order.ShippingAddress),
            Total = order.Total,
            Status = order.Status,
            Date = order.Date
        };

        foreach (var orderItem in order.Items)
        {
            formattedOrderDetails.Items.Add(orderItem);
        }

        return formattedOrderDetails;
    }
}