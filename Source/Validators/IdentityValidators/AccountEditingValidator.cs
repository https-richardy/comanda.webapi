namespace Comanda.WebApi.Validators;

public sealed class AccountEditingValidator :
    AbstractValidator<AccountEditingRequest>,
    IValidator<AccountEditingRequest>
{
    public AccountEditingValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Full name is required.")
            .MinimumLength(3).WithMessage("Full name must be at least 3 characters.");

        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}