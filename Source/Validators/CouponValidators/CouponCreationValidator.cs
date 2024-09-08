namespace Comanda.WebApi.Validators;

public sealed class CouponCreationValidator :
    AbstractValidator<CouponCreationRequest>,
    IValidator<CouponCreationRequest>
{
    public CouponCreationValidator()
    {
        RuleFor(coupon => coupon.Code)
            .NotEmpty().WithMessage("Coupon code is required.")
            .Length(5, 20).WithMessage("Coupon code must be between 5 and 20 characters.");

        RuleFor(coupon => coupon.Discount)
            .GreaterThan(0).WithMessage("Discount must be greater than 0.");

        RuleFor(coupon => coupon.Type)
            .IsInEnum().WithMessage("Coupon type is invalid.");
    }
}