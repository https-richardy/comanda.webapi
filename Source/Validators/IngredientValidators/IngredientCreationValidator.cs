namespace Comanda.WebApi.Validators;

public sealed class IngredientCreationValidator :
    AbstractValidator<IngredientCreationRequest>,
    IValidator<IngredientCreationRequest>
{
    public IngredientCreationValidator()
    {
        RuleFor(ingredient => ingredient.Name)
            .NotEmpty().WithMessage("Name is required.");
    }
}