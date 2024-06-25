namespace Comanda.WebApi.Handlers;

public sealed class AccountRegistrationHandler(
    UserManager<Account> userManager,
    RoleManager<IdentityRole> roleManager,
    IValidator<AccountRegistrationRequest> validator,

    ICustomerRepository customerRepository,
    IEstablishmentOwnerRepository establishmentOwnerRepository
) : IRequestHandler<AccountRegistrationRequest, Response>
{
    public async Task<Response> Handle(AccountRegistrationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        await RegisterAccountAsync(request);
        return new Response(statusCode: 201, message: "Account created successfully.");
    }

    private async Task RegisterAccountAsync(AccountRegistrationRequest request)
    {
        if (request.AccountType == EAccountType.Customer)
            await RegisterCustomerAsync(request);

        if (request.AccountType == EAccountType.EstablishmentOwner)
            await RegisterEstablishmentOwner(request);
    }

    private async Task RegisterCustomerAsync(AccountRegistrationRequest request)
    {
        var account = TinyMapper.Map<Account>(request);
        var customer = new Customer { Account = account, FullName = request.Name };

        await userManager.CreateAsync(account, request.Password);
        await customerRepository.SaveAsync(customer);

        if (!await roleManager.RoleExistsAsync("Customer"))
            await roleManager.CreateAsync(new IdentityRole("Customer"));

        await userManager.AddToRoleAsync(account, "Customer");
    }

    private async Task RegisterEstablishmentOwner(AccountRegistrationRequest request)
    {
        var account = TinyMapper.Map<Account>(request);
        var establishmentOwner = new EstablishmentOwner { Account = account, FullName = request.Name };

        await userManager.CreateAsync(account, request.Password);
        await establishmentOwnerRepository.SaveAsync(establishmentOwner);

        if (!await roleManager.RoleExistsAsync("EstablishmentOwner"))
            await roleManager.CreateAsync(new IdentityRole("EstablishmentOwner"));

        await userManager.AddToRoleAsync(account, "EstablishmentOwner");
    }
}