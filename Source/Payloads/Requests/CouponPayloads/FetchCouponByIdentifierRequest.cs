namespace Comanda.WebApi.Payloads;

public sealed record FetchCouponByIdentifier : IRequest<Response<Coupon>>
{
    public int Id { get; init; }
}