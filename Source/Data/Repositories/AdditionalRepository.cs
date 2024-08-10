namespace Comanda.WebApi.Data.Repositories;

public sealed class AdditionalRepository(ComandaDbContext dbContext) :
    Repository<Additional, ComandaDbContext>(dbContext),
    IAdditionalRepository
{
    public async override Task<IEnumerable<Additional>> RetrieveAllAsync()
    {
        return await _dbContext.Additionals
            .AsNoTracking()
            .Include(additional => additional.Category)
            .ToListAsync();
    }
}