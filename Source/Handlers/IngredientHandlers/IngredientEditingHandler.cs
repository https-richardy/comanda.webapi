namespace Comanda.WebApi.Handlers;

public sealed class IngredientEditingHandler(
    IIngredientRepository ingredientRepository,
    IValidator<IngredientEditingRequest> validator
) : IRequestHandler<IngredientEditingRequest, Response>
{
    public async Task<Response> Handle(
        IngredientEditingRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var ingredient = await ingredientRepository.RetrieveByIdAsync(request.IngredientId);
        if (ingredient is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Ingredient not found."
            );

        ingredient.Name = request.Name;
        await ingredientRepository.UpdateAsync(ingredient);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Ingredient updated successfully."
        );
    }
}