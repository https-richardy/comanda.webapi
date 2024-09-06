namespace Comanda.WebApi.Handlers;

public sealed class AccountRegistrationHandler(
    UserManager<Account> userManager,
    RoleManager<IdentityRole> roleManager,
    IValidator<AccountRegistrationRequest> validator,
    ICustomerRepository customerRepository
) : IRequestHandler<AccountRegistrationRequest, Response>
{
    public async Task<Response> Handle(AccountRegistrationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var existingAccount = await userManager.FindByEmailAsync(request.Email);
        if (existingAccount is not null)
            return new Response(
                statusCode: StatusCodes.Status409Conflict,
                message: "Account with this email already exists."
            );

        await PerformAccountRegistration(request);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Account created successfully."
        );
    }

    private async Task PerformAccountRegistration(AccountRegistrationRequest request)
    {
        var account = TinyMapper.Map<Account>(request);
        var customer = new Customer { Account = account, FullName = request.Name };

        await userManager.CreateAsync(account, request.Password);
        await customerRepository.SaveAsync(customer);

        if (!await roleManager.RoleExistsAsync("Customer"))
            await roleManager.CreateAsync(new IdentityRole("Customer"));

        await userManager.AddToRoleAsync(account, "Customer");
    }
}