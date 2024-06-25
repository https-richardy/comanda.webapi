namespace Comanda.WebApi.Data.Repositories;

public sealed class EstablishmentRepository(ComandaDbContext dbContext) :
    Repository<Establishment, ComandaDbContext>(dbContext),
    IEstablishmentRepository
{
    public async Task AddProductAsync(Establishment establishment, Product product)
    {
        establishment.Products.Add(product);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCategoryAsync(Establishment establishment, Category category)
    {
        establishment.Categories.Add(category);
        await _dbContext.SaveChangesAsync();
    }
}