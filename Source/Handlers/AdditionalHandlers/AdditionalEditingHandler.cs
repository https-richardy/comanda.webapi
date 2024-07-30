namespace Comanda.WebApi.Handlers;

public sealed class AdditionalEditingHandler(
    IAdditionalRepository additionalRepository,
    ICategoryRepository categoryRepository,
    IValidator<AdditionalEditingRequest> validator
) : IRequestHandler<AdditionalEditingRequest, Response>
{
    public async Task<Response> Handle(
        AdditionalEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var existingAdditional = await additionalRepository.RetrieveByIdAsync(request.AdditionalId);
        if (existingAdditional is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Additional not found."
            );

        var category = await categoryRepository.RetrieveByIdAsync(request.CategoryId);
        if (category is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Category not found."
            );

        var updatedAdditional = TinyMapper.Map<Additional>(request);
        updatedAdditional.Category = category;

        await additionalRepository.UpdateAsync(updatedAdditional);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Additional updated successfully."
        );
    }
}