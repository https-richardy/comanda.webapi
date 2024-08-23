namespace Comanda.WebApi.Handlers;

public sealed class SendPasswordResetTokenHandler(
    UserManager<Account> userManager,
    IWebHostEnvironment environment,
    IEmailService emailService,
    IConfirmationTokenService tokenService,
    IValidator<SendPasswordResetTokenRequest> validator
) : IRequestHandler<SendPasswordResetTokenRequest, Response>
{
    public async Task<Response> Handle(
        SendPasswordResetTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "User not found."
            );

        var token = tokenService.GenerateToken();

        user.ConfirmationToken = token;
        await userManager.UpdateAsync(user);

        var emailBody = await BuildEmailTemplateAsync(token);
        await emailService.SendEmailAsync(
            to: user.Email!,
            subject: "Esqueceu sua Senha?",
            body: emailBody
        );

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "If your email is valid, a password reset link has been sent."
        );
    }

    private async Task<string> BuildEmailTemplateAsync(ConfirmationToken confirmationToken)
    {
        var filePath = Path.Combine(environment.WebRootPath, "password-reset.html");
        var emailTemplate = await File.ReadAllTextAsync(filePath);

        return emailTemplate.Replace("{{TOKEN}}", confirmationToken.Token);
    }
}
