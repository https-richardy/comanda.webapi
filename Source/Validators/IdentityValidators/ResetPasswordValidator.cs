namespace Comanda.WebApi.Validators;

public sealed class ResetPasswordValidator :
    AbstractValidator<ResetPasswordRequest>,
    IValidator<ResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");

        RuleFor(request => request.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(request => request.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")
            .WithMessage("New password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
    }
}