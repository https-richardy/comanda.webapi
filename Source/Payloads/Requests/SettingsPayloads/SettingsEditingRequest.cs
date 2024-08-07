namespace Comanda.WebApi.Payloads;

public sealed record SettingsEditingRequest : IRequest<Response>
{
    public bool AcceptAutomatically { get; init; }
    public int MaxConcurrentAutomaticOrders { get; init; }
    public int EstimatedDeliveryTimeInMinutes { get; init; }
    public decimal DeliveryFee { get; init; }
}