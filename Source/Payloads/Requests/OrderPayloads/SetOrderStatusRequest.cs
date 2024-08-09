namespace Comanda.WebApi.Payloads;

public sealed record SetOrderStatusRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int OrderId { get; init; }
    public EOrderStatus Status { get; init; }
}