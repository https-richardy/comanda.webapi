namespace Comanda.WebApi.Services;

public sealed class SummaryService : ISummaryService
{
    private readonly ComandaDbContext _dbContext;

    public SummaryService(ComandaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DailySummary> GetDailySummaryAsync()
    {
        var today = DateTime.Today;

        var totalOrders = await _dbContext.Orders
            .Where(order => order.Date.Date == today)
            .ToListAsync();

        var processedOrders = totalOrders
            .Where(order => order.Status == EOrderStatus.Delivered)
            .ToList();

        var processedOrdersCount = processedOrders.Count;
        var totalRevenue =
            processedOrders.Any() ?
            processedOrders.Sum(order => order.Total) : 0;

        var averageOrderValue =
            processedOrders.Any() ?
            processedOrders.Average(order => order.Total) : 0;

        var largestOrder =
            processedOrders.Any() ?
            processedOrders.Max(order => order.Total) : 0;

        Expression<Func<Order, bool>> cancelledOrdersPredicate = order =>
            order.Status == EOrderStatus.CancelledByCustomer ||
            order.Status == EOrderStatus.CancelledBySystem &&
            order.Date.Date == today;

        var cancelledOrders = await _dbContext.Orders
            .Where(cancelledOrdersPredicate)
            .CountAsync();

        return new DailySummary
        {
            TotalRevenue = totalRevenue,
            ProcessedOrders = processedOrdersCount,
            CancelledOrders = cancelledOrders,
            AverageOrderValue = averageOrderValue,
            LargestOrder = largestOrder
        };
    }
}