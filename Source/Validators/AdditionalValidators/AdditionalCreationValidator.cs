namespace Comanda.WebApi.Validators;

public sealed class AdditionalCreationValidator :
    AbstractValidator<AdditionalCreationRequest>,
    IValidator<AdditionalCreationRequest>
{
    public AdditionalCreationValidator()
    {
        RuleFor(additional => additional.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Name must be at most 50 characters.");

        RuleFor(additional => additional.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than zero.")
            .LessThan(1000).WithMessage("Price must be less than 1000.");

        RuleFor(additional => additional.CategoryId)
            .GreaterThan(0).WithMessage("Category id must be greater than zero.");
    }
}