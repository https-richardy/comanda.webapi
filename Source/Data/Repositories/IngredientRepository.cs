namespace Comanda.WebApi.Data.Repositories;

public sealed class IngredientRepository(ComandaDbContext dbContext) :
    MinimalRepository<Ingredient, ComandaDbContext>(dbContext),
    IIngredientRepository
{

}