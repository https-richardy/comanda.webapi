namespace Comanda.WebApi.Validators;

public sealed class UpdateItemQuantityValidator : 
    AbstractValidator<UpdateItemQuantityInCartRequest>, IValidator<UpdateItemQuantityInCartRequest>
{
    public UpdateItemQuantityValidator()
    {
        RuleFor(request => request.ProductId)
            .NotEmpty().WithMessage("Product ID is required.")
            .GreaterThan(0).WithMessage("Product ID must be greater than 0.");

        RuleFor(request => request.NewQuantity)
            .NotEmpty().WithMessage("Quantity is required.");
    }
}