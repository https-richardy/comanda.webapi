namespace Comanda.WebApi.Data.Repositories;

public sealed class OrderRepository(ComandaDbContext dbContext) :
    Repository<Order, ComandaDbContext>(dbContext),
    IOrderRepository
{
    public override async Task<IEnumerable<Order>> PagedAsync(int pageNumber, int pageSize)
    {
        /* Represents the adjustment applied to page numbers to align with zero-based indices in LINQ queries. */
        const int pageIndexAdjustment = 1;

        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.ShippingAddress)
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .Skip((pageNumber - pageIndexAdjustment) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Order>> PagedAsync(Expression<Func<Order, bool>> predicate, int pageNumber, int pageSize)
    {
        /* Represents the adjustment applied to page numbers to align with zero-based indices in LINQ queries. */
        const int pageIndexAdjustment = 1;

        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.ShippingAddress)
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .Where(predicate)
            .Skip((pageNumber - pageIndexAdjustment) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    #pragma warning disable CS8603
    public override async Task<Order> RetrieveByIdAsync(int id)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.ShippingAddress)
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .Include(order => order.Items)
            .ThenInclude(item => item.UnselectedIngredients)
            .ThenInclude(unselectedIngredient => unselectedIngredient.Ingredient)
            .Include(order => order.Items)
            .ThenInclude(item => item.Additionals)
            .ThenInclude(additional => additional.Additional)
            .FirstOrDefaultAsync(order => order.Id == id);
    }
}
