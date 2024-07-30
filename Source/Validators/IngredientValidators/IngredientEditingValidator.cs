namespace Comanda.WebApi.Validators;

public sealed class IngredientEditingValidator :
    AbstractValidator<IngredientEditingRequest>,
    IValidator<IngredientEditingRequest>
{
    public IngredientEditingValidator()
    {
        RuleFor(request => request.IngredientId)
            .NotEmpty()
            .WithMessage("Ingredient ID is required.")
            .GreaterThan(0)
            .WithMessage("Ingredient ID must be greater than 0.");

        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}