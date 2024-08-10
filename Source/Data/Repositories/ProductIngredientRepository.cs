namespace Comanda.WebApi.Data.Repositories;

public sealed class ProductIngredientRepository(ComandaDbContext dbContext) :
    MinimalRepository<ProductIngredient, ComandaDbContext>(dbContext),
    IProductIngredientRepository
{
    #pragma warning disable CS8603
    public override async Task<ProductIngredient> RetrieveByIdAsync(int id)
    {
        return await _dbContext.ProductIngredients
            .AsNoTracking()
            .Include(productIngredient => productIngredient.Ingredient)
            .Where(productIgredient => productIgredient.Id == id)
            .FirstOrDefaultAsync();
    }
}