namespace Comanda.WebApi.Validators;

public sealed class CreateEstablishmentProductValidator :
    AbstractValidator<CreateEstablishmentProductRequest>,
    IValidator<CreateEstablishmentProductRequest>
{
    public CreateEstablishmentProductValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(5, 100).WithMessage("Title must be between 5 and 100 characters.");

        RuleFor(request => request.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(10, 500).WithMessage("Description must be between 10 and 500 characters.");

        RuleFor(request => request.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

    }
}