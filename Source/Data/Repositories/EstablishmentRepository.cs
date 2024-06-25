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

    public async Task<bool> CategoryExistsAsync(int categoryId)
    {
        return await _dbContext.Categories.AnyAsync(category => category.Id == categoryId);
    }

    #pragma warning disable CS8603
    public async Task<EstablishmentOwner> FindOwnerAsync(int establishmentId)
    {
        return await _dbContext.Establishments
            .Where(establishment => establishment.Id == establishmentId)
            .Select(establishment => establishment.Owner)
            .FirstOrDefaultAsync();
    }
}