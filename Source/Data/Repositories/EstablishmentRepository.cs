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
}