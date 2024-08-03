namespace Comanda.WebApi.Payloads;

public sealed record CartItemDeletionRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int ItemId { get; set; }
}