namespace Comanda.WebApi.Data.Repositories;

public sealed class AdditionalRepository(ComandaDbContext dbContext) :
    MinimalRepository<Additional, ComandaDbContext>(dbContext),
    IAdditionalRepository
{
    public async override Task<IEnumerable<Additional>> RetrieveAllAsync()
    {
        return await _dbContext.Additionals
            .Include(cart => cart.Category)
            .ToListAsync();
    }
}