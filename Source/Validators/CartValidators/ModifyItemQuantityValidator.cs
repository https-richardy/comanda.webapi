namespace Comanda.WebApi.Validators;

public sealed class ModifyItemQuantityValidator :
    AbstractValidator<ModifyCartItemQuantityRequest>, IValidator<ModifyCartItemQuantityRequest>
{
    public ModifyItemQuantityValidator()
    {
        RuleFor(request => request.ProductId)
            .NotEmpty()
            .WithMessage("Product id is required.")
            .GreaterThan(0)
            .WithMessage("Product id must be greater than 0.");

        RuleFor(request => request.Action)
            .IsInEnum()
            .WithMessage("Invalid action. The action must be increment (0) or decrement (1).");
    }
}