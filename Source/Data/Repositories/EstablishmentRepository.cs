namespace Comanda.WebApi.Data.Repositories;

public sealed class EstablishmentRepository(ComandaDbContext dbContext) :
    Repository<Establishment, ComandaDbContext>(dbContext),
    IEstablishmentRepository
{
    #pragma warning disable CS8603
    public async Task<IEnumerable<Product>> GetProductsAsync(int pageNumber, int pageSize, int establishmentId)
    {
        var establishment = await _dbContext.Establishments
            .Include(establishment => establishment.Products)
            .FirstOrDefaultAsync(establishment => establishment.Id == establishmentId);

        /* Represents the adjustment applied to page numbers to align with zero-based indices in LINQ queries. */
        const int pageIndexAdjustment = 1;

        var products = establishment?.Products
            .Skip((pageNumber - pageIndexAdjustment) * pageSize)
            .Take(pageSize)
            .ToList();

        return products;
    }

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

    #pragma warning disable CS8603
    public async Task<Category> RetrieveCategoryByIdAsync(int categoryId)
    {
        return await _dbContext.Categories
            .Where(category => category.Id == categoryId)
            .FirstOrDefaultAsync();
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