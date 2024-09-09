namespace Comanda.WebApi.Payloads;

public sealed record FetchCouponByCodeRequest : IRequest<Response<Coupon>>
{
    public required string Code { get; init; }
}