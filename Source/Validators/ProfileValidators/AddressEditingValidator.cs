namespace Comanda.WebApi.Validators;

public sealed class AddressEditingValidator :
    AbstractValidator<AddressEditingRequest>,
    IValidator<AddressEditingRequest>
{
    public AddressEditingValidator()
    {
    RuleFor(address => address.AddressId)
        .NotEmpty().WithMessage("Address ID is required.")
        .GreaterThan(0).WithMessage("Address ID must be greater than zero.");

        RuleFor(address => address.PostalCode)
            .NotEmpty().WithMessage("Postal code cannot be empty.")
            .Length(8).WithMessage("Postal code must be exactly 8 digits long.")
            .Matches("^[0-9]{8}$").WithMessage("Postal code must be in the format 12345678.");
    }
}