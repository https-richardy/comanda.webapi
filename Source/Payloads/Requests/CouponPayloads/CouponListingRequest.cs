namespace Comanda.WebApi.Payloads;

public sealed record CouponListingRequest : IRequest<Response<IEnumerable<Coupon>>>
{

}