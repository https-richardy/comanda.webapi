namespace Comanda.WebApi.Handlers;

public sealed class AccountEditingHandler(
    UserManager<Account> userManager,
    IUserContextService userContextService,
    IValidator<AccountEditingRequest> validator
) : IRequestHandler<AccountEditingRequest, Response>
{
    public async Task<Response> Handle(
        AccountEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var userIdentifier = userContextService.GetCurrentUserIdentifier();
        var user = await userManager.FindByIdAsync(userIdentifier!);

        #pragma warning disable CS8602

        user.Email = request.Email;
        user.UserName = request.Name;

        await userManager.UpdateAsync(user);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Account updated successfully."
        );
    }
}