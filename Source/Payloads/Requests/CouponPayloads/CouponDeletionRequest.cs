namespace Comanda.WebApi.Payloads;

public sealed record CouponDeletionRequest : IRequest<Response>
{
    [JsonIgnore] /* this property will be set from the route. */
    public int Id { get; init; }
}