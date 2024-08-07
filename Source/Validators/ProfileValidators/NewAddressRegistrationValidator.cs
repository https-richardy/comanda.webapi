namespace Comanda.WebApi.Validators;

public sealed class NewAddressRegistrationValidators :
    AbstractValidator<NewAddressRegistrationRequest>,
    IValidator<NewAddressRegistrationRequest>
{
    public NewAddressRegistrationValidators()
    {
        RuleFor(address => address.PostalCode)
            .NotEmpty().WithMessage("Postal code cannot be empty.")
            .Length(8).WithMessage("Postal code must be exactly 8 digits long.")
            .Matches("^[0-9]{8}$").WithMessage("Postal code must be in the format 12345678.");
    }
}
