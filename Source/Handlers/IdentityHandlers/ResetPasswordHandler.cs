namespace Comanda.WebApi.Handlers;

public sealed class ResetPasswordHandler(
    UserManager<Account> userManager,
    IPasswordHasher<Account> passwordHasher,
    IValidator<ResetPasswordRequest> validator
) : IRequestHandler<ResetPasswordRequest, Response>
{
    public async Task<Response> Handle(
        ResetPasswordRequest request,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.Users
            .Include(user => user.ConfirmationToken)
            .FirstOrDefaultAsync(user => user.Email == request.Email);

        if (user == null)
            return new Response
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "User not found."
            };

        if (user.ConfirmationToken! == null! || user.ConfirmationToken.Token != request.Token || user.ConfirmationToken.ExpirationDate < DateTime.Now)
            return new Response
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Invalid or expired token."
            };

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
        user.ConfirmationToken = null;

        await userManager.UpdateAsync(user);

        return new Response
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Password reset successful."
        };
    }
}