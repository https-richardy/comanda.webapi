namespace Comanda.WebApi.Handlers;

public sealed class CouponListingHandler(ICouponRepository repository) :
    IRequestHandler<CouponListingRequest, Response<IEnumerable<Coupon>>>
{
    public async Task<Response<IEnumerable<Coupon>>> Handle(
        CouponListingRequest request,
        CancellationToken cancellationToken
    )
    {
        var coupons = await repository.RetrieveAllAsync();

        return new Response<IEnumerable<Coupon>>(
            data: coupons,
            statusCode: StatusCodes.Status200OK,
            message: "Coupons retrieved successfully."
        );
    }
}
