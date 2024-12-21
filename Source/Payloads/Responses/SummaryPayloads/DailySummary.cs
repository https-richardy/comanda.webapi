namespace Comanda.WebApi.Payloads;

public sealed record DailySummary
{
    public decimal TotalRevenue { get; init; }
    public int ProcessedOrders { get; init; }
    public int CancelledOrders { get; init; }
    public decimal AverageOrderValue { get; init; }
    public decimal LargestOrder { get; init; }
}