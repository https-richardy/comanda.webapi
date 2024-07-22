namespace Comanda.WebApi.Data.Repositories;

public sealed class ProductRepository(ComandaDbContext dbContext) :
    Repository<Product, ComandaDbContext>(dbContext),
    IProductRepository
{

}