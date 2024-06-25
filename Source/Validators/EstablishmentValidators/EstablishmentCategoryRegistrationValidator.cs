namespace Comanda.WebApi.Validators;

public sealed class EstablishmentCategoryRegistrationValidator :
    AbstractValidator<EstablishmentCategoryRegistrationRequest>,
    IValidator<EstablishmentCategoryRegistrationRequest>
{
    public EstablishmentCategoryRegistrationValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(50).WithMessage("Category name must not exceed 50 characters.");
    }
}