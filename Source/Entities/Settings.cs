namespace Comanda.WebApi.Entities;

public sealed class Settings : Entity
{
    public bool AcceptAutomatically { get; set; }
    public int MaxConcurrentAutomaticOrders { get; set; }
    public int EstimatedDeliveryTimeInMinutes { get; set; }
    public decimal DeliveryFee { get; set; }
}