namespace Comanda.WebApi.Handlers;

public sealed class CouponCreationHandler(
    ICouponService couponService,
    IValidator<CouponCreationRequest> validator
) : IRequestHandler<CouponCreationRequest, Response>
{
    public async Task<Response> Handle(CouponCreationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var coupon = (Coupon)request;
        await couponService.AddCouponAsync(coupon);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Coupon created successfully."
        );
    }
}
