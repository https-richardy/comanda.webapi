namespace Comanda.WebApi.Handlers;

public sealed class CreateEstablishmentHandler(
    UserManager<Account> userManager,
    IValidator<CreateEstablishmentRequest> validator,
    IEstablishmentRepository establishmentRepository
) : IRequestHandler<CreateEstablishmentRequest, Response>
{
    public async Task<Response> Handle(CreateEstablishmentRequest request, CancellationToken cancellationToken)
    {
        var account = await userManager.FindByIdAsync(request.UserId);
        if (account is null)
            return new Response(statusCode: 404, message: "user not found");

        if (!await userManager.IsInRoleAsync(account, "EstablishmentOwner"))
            return new Response(statusCode: 403, message: "user is not an establishment owner. Only establishment owners can create an establishment.");

        if (!await CanCreateEstablishment(account))
            return new Response(statusCode: 403, message: "user already has an establishment.");

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var establishment = TinyMapper.Map<Establishment>(request);
        establishment.Owner.Account = account;

        await establishmentRepository.SaveAsync(establishment);
        return new Response(statusCode: 201, message: "establishment created successfully.");
    }

    private async Task<bool> CanCreateEstablishment(Account account)
    {
        /* for now, "EstablishmentOwner" type accounts can only have 1 establishment associated with their account. */
        var count = await establishmentRepository.CountAsync(establishment => establishment.Owner.Account.Id == account.Id);
        return count == 0;
    }
}