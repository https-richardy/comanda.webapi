namespace Comanda.WebApi.Data.Repositories;

public sealed class CategoryRepository(ComandaDbContext dbContext) :
    Repository<Category, ComandaDbContext>(dbContext),
    ICategoryRepository
{

}