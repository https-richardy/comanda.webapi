namespace Comanda.WebApi.Validators;

public sealed class CreateEstablishmentValidator :
    AbstractValidator<CreateEstablishmentRequest>,
    IValidator<CreateEstablishmentRequest>
{
    public CreateEstablishmentValidator()
    {
        RuleFor(request => request.EstablishmentName)
            .NotEmpty()
            .WithMessage("Establishment name is required.")
            .MaximumLength(100)
            .WithMessage("Establishment name must not exceed 100 characters.");

        RuleFor(request => request.PostalCode)
            .NotEmpty()
            .WithMessage("Postal code is required.")
            .MaximumLength(8)
            .WithMessage("Postal code must not exceed 8 characters.");
    }
}