namespace Comanda.WebApi.Validators;

public sealed class CouponEditingValidator :
    AbstractValidator<CouponEditingRequest>,
    IValidator<CouponEditingRequest>
{
    public CouponEditingValidator()
    {
        RuleFor(coupon => coupon.Id)
            .GreaterThan(0).WithMessage("Coupon ID must be greater than 0.");

        RuleFor(coupon => coupon.Code)
            .NotEmpty().WithMessage("Coupon code is required.")
            .Length(5, 20).WithMessage("Coupon code must be between 5 and 20 characters.");

        RuleFor(coupon => coupon.Discount)
            .GreaterThan(0).WithMessage("Discount must be greater than 0.");

        RuleFor(coupon => coupon.Type)
            .IsInEnum().WithMessage("Coupon type is invalid.");

        RuleFor(coupon => coupon.ExpirationDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Expiration date must be in the future.");
    }
}