namespace Comanda.WebApi.Payloads;

public sealed record OrderCancellationRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int OrderId { get; set; }
}