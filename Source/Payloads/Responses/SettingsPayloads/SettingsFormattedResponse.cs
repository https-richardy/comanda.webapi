namespace Comanda.WebApi.Payloads;

public sealed record SettingsFormattedResponse
{
    public bool AcceptAutomatically { get; set; }
    public int MaxConcurrentAutomaticOrders { get; set; }
    public int EstimatedDeliveryTimeInMinutes { get; set; }
    public decimal DeliveryFee { get; set; }

    public SettingsFormattedResponse(
        bool acceptAutomatically,
        int maxConcurrentAutomaticOrders,
        int estimatedDeliveryTimeInMinutes,
        decimal deliveryFee
    )
    {
        AcceptAutomatically = acceptAutomatically;
        MaxConcurrentAutomaticOrders = maxConcurrentAutomaticOrders;
        EstimatedDeliveryTimeInMinutes = estimatedDeliveryTimeInMinutes;
        DeliveryFee = deliveryFee;
    }
}