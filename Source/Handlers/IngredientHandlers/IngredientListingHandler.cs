namespace Comanda.WebApi.Handlers;

public sealed class IngredientListingHandler(
    IIngredientRepository ingredientRepository
) : IRequestHandler<IngredientListingRequest, Response<IEnumerable<BasicIngredientDetail>>>
{
    public async Task<Response<IEnumerable<BasicIngredientDetail>>> Handle(
        IngredientListingRequest request,
        CancellationToken cancellationToken
    )
    {
        var ingredients = await ingredientRepository.RetrieveAllAsync();
        var payload = ingredients
            .Select(ingredient => TinyMapper.Map<BasicIngredientDetail>(ingredient))
            .ToList();

        return new Response<IEnumerable<BasicIngredientDetail>>(
            data: payload,
            statusCode: StatusCodes.Status200OK,
            message: "Ingredients retrieved successfully."
        );
    }
}