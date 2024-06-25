#pragma warning disable CS8602

namespace Comanda.WebApi.Handlers;

public sealed class CreateEstablishmentProductHandler(
    UserManager<Account> userManager,
    IFileUploadService fileUploadService,
    IEstablishmentRepository establishmentRepository,
    IValidator<CreateEstablishmentProductRequest> validator
) : IRequestHandler<CreateEstablishmentProductRequest, Response>
{
    public async Task<Response> Handle(CreateEstablishmentProductRequest request, CancellationToken cancellationToken)
    {
        var account = await userManager.FindByIdAsync(request.UserId);

        var establishment = await establishmentRepository.FindSingleAsync(establishment => establishment.Id == request.EstablishmentId);
        if (establishment is null)
            return new Response(statusCode: 404, message: "establishment not found");

        if (establishment.Owner.Account.Id != account.Id)
            return new Response(statusCode: 403, message: "The user is not the owner of this establishment.");

        if (!await CanCreateProductAsync(account))
            return new Response(statusCode: 403, message: "the user does not have an establishment. The user must have a registered establishment before registering a product.");

        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = TinyMapper.Map<Product>(request);
        product.ImagePath = await fileUploadService.UploadFileAsync(request.Image);

        await establishmentRepository.AddProductAsync(establishment, product);

        return new Response(statusCode: 201, message: "product created successfully.");
    }

    private async Task<bool> CanCreateProductAsync(Account account)
    {
        var count = await establishmentRepository.CountAsync(establishment => establishment.Owner.Account.Id == account.Id);
        return count != 1;
    }
}
