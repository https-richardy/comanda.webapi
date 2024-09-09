namespace Comanda.WebApi.Handlers;

public sealed class CouponEditingHandler(
    ICouponService couponService,
    ICouponRepository couponRepository,
    IValidator<CouponEditingRequest> validator
) : IRequestHandler<CouponEditingRequest, Response>
{
    public async Task<Response> Handle(
        CouponEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var existingCoupon = await couponRepository.RetrieveByIdAsync(request.Id);
        if (existingCoupon is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Coupon not found."
            );

        var coupon = (Coupon)request;
        await couponService.UpdateCouponAsync(coupon);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Coupon updated successfully."
        );
    }
}
