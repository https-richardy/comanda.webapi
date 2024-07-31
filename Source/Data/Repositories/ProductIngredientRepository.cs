namespace Comanda.WebApi.Data.Repositories;

public sealed class ProductIngredientRepository(ComandaDbContext dbContext) :
    MinimalRepository<ProductIngredient, ComandaDbContext>(dbContext),
    IProductIngredientRepository
{

}