namespace Comanda.WebApi.Handlers;

public sealed class IngredientDeletionHandler(
    IIngredientRepository ingredientRepository
) : IRequestHandler<IngredientDeletionRequest, Response>
{
    public async Task<Response> Handle(
        IngredientDeletionRequest request,
        CancellationToken cancellationToken
    )
    {
        var ingredient = await ingredientRepository.RetrieveByIdAsync(request.IngredientId);
        if (ingredient is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "Ingredient not found."
            );

        await ingredientRepository.DeleteAsync(ingredient);
        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "Ingredient deleted successfully."
        );
    }
}