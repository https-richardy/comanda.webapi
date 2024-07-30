namespace Comanda.WebApi.Validators;

public sealed class AdditionalEditingValidator :
    AbstractValidator<AdditionalEditingRequest>,
    IValidator<AdditionalEditingRequest>
{
    public AdditionalEditingValidator()
    {
        RuleFor(additional => additional.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(additional => additional.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(additional => additional.CategoryId)
            .GreaterThan(0).WithMessage("Category id must be greater than zero.");
    }
}