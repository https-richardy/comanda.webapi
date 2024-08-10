namespace Comanda.WebApi.Payloads;

public sealed record CustomerOrderDetailsRequest :
    IRequest<Response<FormattedOrderDetails>>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int OrderId { get; set; }
}