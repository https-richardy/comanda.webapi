namespace Comanda.WebApi.Handlers;

public sealed class SendPasswordResetTokenHandler(
    UserManager<Account> userManager,
    IHostInformationProvider hostInformation,
    IWebHostEnvironment environment,
    IEmailService emailService,
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

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{hostInformation.HostAddress}/api/identity/reset-password" +
            $"?token={Uri.EscapeDataString(token)}" +
            $"&email={Uri.EscapeDataString(user.Email!)}";

        var emailBody = await BuildEmailTemplateAsync(resetLink);

        await emailService.SendEmailAsync(
            to: user.Email!,
            subject: "Esqueceu sua Senha? Vamos Resolver Isso Juntos!",
            body: emailBody
        );

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "If your email is valid, a password reset link has been sent."
        );
    }

    private async Task<string> BuildEmailTemplateAsync(string resetLink)
    {
        var filePath = Path.Combine(environment.WebRootPath, "password-reset.html");
        var emailTemplate = await File.ReadAllTextAsync(filePath);

        return emailTemplate.Replace("{{ResetLink}}", resetLink);
    }
}
