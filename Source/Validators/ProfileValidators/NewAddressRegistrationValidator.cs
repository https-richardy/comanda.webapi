namespace Comanda.WebApi.Validators;

public sealed class NewAddressRegistrationValidator :
    AbstractValidator<NewAddressRegistrationRequest>,
    IValidator<NewAddressRegistrationRequest>
{
    public NewAddressRegistrationValidator()
    {
        RuleFor(address => address.PostalCode)
            .NotEmpty().WithMessage("Postal code cannot be empty.")
            .Length(8).WithMessage("Postal code must be exactly 8 digits long.")
            .Matches("^[0-9]{8}$").WithMessage("Postal code must be in the format 12345678.");
    }
}
