namespace Comanda.WebApi.Handlers;

public sealed class CouponDeletionHandler(
    ICouponService couponService,
    ICouponRepository couponRepository
) :
    IRequestHandler<CouponDeletionRequest, Response>
{
    public async Task<Response> Handle(
        CouponDeletionRequest request,
        CancellationToken cancellationToken
    )
    {
        var coupon = await couponRepository.RetrieveByIdAsync(request.Id);
        if (coupon is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Coupon not found."
            );

        await couponService.DeleteCouponAsync(coupon);
        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Coupon deleted successfully."
        );
    }
}