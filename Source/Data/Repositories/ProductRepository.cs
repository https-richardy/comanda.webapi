namespace Comanda.WebApi.Data.Repositories;

public sealed class ProductRepository(ComandaDbContext dbContext) :
    Repository<Product, ComandaDbContext>(dbContext),
    IProductRepository
{
    public override  async Task<IEnumerable<Product>> PagedAsync(int pageNumber, int pageSize)
    {
        /* Represents the adjustment applied to page numbers to align with zero-based indices in LINQ queries. */
        const int pageIndexAdjustment = 1;

        return await _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Ingredients)
            .Skip((pageNumber - pageIndexAdjustment) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Product>> PagedAsync(Expression<Func<Product, bool>> predicate, int pageNumber, int pageSize)
    {
        /* Represents the adjustment applied to page numbers to align with zero-based indices in LINQ queries. */
        const int pageIndexAdjustment = 1;

        return await _dbContext.Products
            .Include(product => product.Category)
            .Include(product => product.Ingredients)
            .Where(predicate)
            .Skip((pageNumber - pageIndexAdjustment) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}