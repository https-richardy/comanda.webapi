namespace Comanda.WebApi.Payloads;

public sealed class IngredientListingRequest :
    IRequest<Response<IEnumerable<Ingredient>>>
{

}