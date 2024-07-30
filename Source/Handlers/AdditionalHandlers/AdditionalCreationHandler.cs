namespace Comanda.WebApi.Handlers;

public sealed class AdditionalCreationHandler(
    IAdditionalRepository additionalRepository,
    ICategoryRepository categoryRepository,
    IValidator<AdditionalCreationRequest> validator
) : IRequestHandler<AdditionalCreationRequest, Response>
{
    public async Task<Response> Handle(
        AdditionalCreationRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var existingCategory = await categoryRepository.RetrieveByIdAsync(request.CategoryId);
        if (existingCategory is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        var additional = TinyMapper.Map<Additional>(request);
        additional.Category = existingCategory;

        await additionalRepository.SaveAsync(additional);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Additional created successfully."
        );
    }
}