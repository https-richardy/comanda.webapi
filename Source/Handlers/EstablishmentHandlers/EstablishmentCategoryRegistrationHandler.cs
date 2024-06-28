namespace Comanda.WebApi.Handlers;

public sealed class EstablishmentCategoryRegistrationHandler(
    UserManager<Account> userManager,
    IValidator<EstablishmentCategoryRegistrationRequest> validator,
    IEstablishmentRepository establishmentRepository
) : IRequestHandler<EstablishmentCategoryRegistrationRequest, Response>
{
    public async Task<Response> Handle(
        EstablishmentCategoryRegistrationRequest request,
        CancellationToken cancellationToken
    )
    {
        var account = await userManager.FindByIdAsync(request.UserId);
        if (account is null)
            return new Response(statusCode: 404, message: "user not found");

        var establishment = await establishmentRepository.FindSingleAsync(establishment => establishment.Id == request.EstablishmentId);
        if (establishment is null)
            return new Response(statusCode: 404, message: "establishment not found");

        var owner = await establishmentRepository.FindOwnerAsync(establishment.Id);

        if (owner.Account.Id != account.Id)
            return new Response(statusCode: 403, message: "The user is not the owner of this establishment.");

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var category = TinyMapper.Map<Category>(request);
        await establishmentRepository.AddCategoryAsync(establishment, category);

        return new Response(statusCode: 201, message: "category created successfully.");
    }
}