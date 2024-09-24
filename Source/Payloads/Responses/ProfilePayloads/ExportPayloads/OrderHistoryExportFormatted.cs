namespace Comanda.WebApi.Payloads;

public sealed record OrderHistoryExportFormatted
{
    public int OrderId { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }

    public static implicit operator OrderHistoryExportFormatted(Order order)
    {
        return new OrderHistoryExportFormatted
        {
            OrderId = order.Id,
            OrderDate = order.Date,
            TotalAmount = order.Total
        };
    }
}