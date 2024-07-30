namespace Comanda.WebApi.Handlers;

public sealed class IngredientCreationHandler(
    IIngredientRepository ingredientRepository,
    IValidator<IngredientCreationRequest> validator
) : IRequestHandler<IngredientCreationRequest, Response>
{
    public async Task<Response> Handle(
        IngredientCreationRequest request,
        CancellationToken cancellationToken
    )
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ValidationFailureResponse(errors: validationResult.Errors);

        var ingredient = new Ingredient { Name = request.Name };
        await ingredientRepository.SaveAsync(ingredient);

        return new Response(
            statusCode: StatusCodes.Status201Created,
            message: "Ingredient created successfully."
        );
    }
}