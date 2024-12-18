namespace Comanda.WebApi.Payloads;

public sealed class OrderConfirmation
{
    public int OrderId { get; init; }
    public int EstimatedDeliveryTimeInMinutes { get; init; }
    public decimal TotalPaid { get; init; }
}