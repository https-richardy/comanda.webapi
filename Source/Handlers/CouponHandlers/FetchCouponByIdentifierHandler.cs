namespace Comanda.WebApi.Handlers;

public sealed class FetchCouponByIdentifierHandler(ICouponRepository couponRepository) :
    IRequestHandler<FetchCouponByIdentifier, Response<Coupon>>
{
    public async Task<Response<Coupon>> Handle(
        FetchCouponByIdentifier request,
        CancellationToken cancellationToken
    )
    {
        var coupon = await couponRepository.RetrieveByIdAsync(request.Id);
        if (coupon is null)
            return new Response<Coupon>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "Coupon not found."
            );

        return new Response<Coupon>(
            data: coupon,
            statusCode: StatusCodes.Status200OK,
            message: "Coupon retrieved successfully."
        );
    }
}