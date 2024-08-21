namespace Comanda.WebApi.Validators;

public sealed class SendPasswordResetTokenValidator :
    AbstractValidator<SendPasswordResetTokenRequest>,
    IValidator<SendPasswordResetTokenRequest>
{
    public SendPasswordResetTokenValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}