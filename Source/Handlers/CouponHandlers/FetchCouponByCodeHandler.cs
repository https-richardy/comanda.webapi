namespace Comanda.WebApi.Handlers;

public sealed class FetchCouponByCodeHandler(ICouponService couponService) :
    IRequestHandler<FetchCouponByCodeRequest, Response<Coupon>>
{
    public async Task<Response<Coupon>> Handle(
        FetchCouponByCodeRequest request,
        CancellationToken cancellationToken
    )
    {
        var coupon = await couponService.GetCouponByCodeAsync(request.Code);
        if (coupon is null)
            return new Response<Coupon>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: $"Coupon with code '{request.Code}' not found."
            );

        return new Response<Coupon>(
            data: coupon,
            statusCode: StatusCodes.Status200OK,
            message: "Coupon retrieved successfully."
        );
    }
}