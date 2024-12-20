namespace Comanda.WebApi.Validators;

public sealed class IngredientCreationValidator :
    AbstractValidator<IngredientCreationRequest>,
    IValidator<IngredientCreationRequest>
{
    public IngredientCreationValidator()
    {
        RuleFor(ingredient => ingredient.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Name must be at most 50 characters.");
    }
}