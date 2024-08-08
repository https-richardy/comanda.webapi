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
            .Include(order => order.Customer)
            .Include(order => order.ShippingAddress)
            .Include(order => order.Items)
            .ThenInclude(item => item.Product)
            .Skip((pageNumber - pageIndexAdjustment) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
