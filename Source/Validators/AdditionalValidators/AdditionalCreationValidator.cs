namespace Comanda.WebApi.Validators;

public sealed class AdditionalCreationValidator :
    AbstractValidator<AdditionalCreationRequest>,
    IValidator<AdditionalCreationRequest>
{
    public AdditionalCreationValidator()
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