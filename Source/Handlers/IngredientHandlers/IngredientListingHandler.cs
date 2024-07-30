namespace Comanda.WebApi.Handlers;

public sealed class IngredientListingHandler(
    IIngredientRepository ingredientRepository
) : IRequestHandler<IngredientListingRequest, Response<IEnumerable<Ingredient>>>
{
    public async Task<Response<IEnumerable<Ingredient>>> Handle(
        IngredientListingRequest request,
        CancellationToken cancellationToken
    )
    {
        var ingredients = await ingredientRepository.RetrieveAllAsync();

        return new Response<IEnumerable<Ingredient>>(
            data: ingredients,
            statusCode: StatusCodes.Status200OK,
            message: "Ingredients retrieved successfully."
        );
    }
}