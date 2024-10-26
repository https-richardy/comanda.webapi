namespace Comanda.WebApi.Data.Repositories;

public sealed class OrderRepository(ComandaDbContext dbContext) :
    Repository<Order, ComandaDbContext>(dbContext),
    IOrderRepository
{
    public override async Task SaveAsync(Order entity)
    {
        // Ensure the entity state is set to Added to avoid the IDENTITY_INSERT error
        // when saving a new Order entity without explicitly setting the ID.

        _dbContext.Entry(entity).State = EntityState.Added;

        foreach (var item in entity.Items)
        {
            _dbContext.Entry(item).State = EntityState.Added;
        }

        await _dbContext.SaveChangesAsync();
    }

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
            .Include(order => order.Items)
            .ThenInclude(item => item.Additionals)
            .ThenInclude(additional => additional.Additional)
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
