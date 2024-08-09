namespace Comanda.WebApi.Payloads;

public sealed record CustomerOrderHistoryRequest :
    IRequest<Response<PaginationHelper<FormattedOrder>>>
{
    public int PageSize { get; init; } = 10;
    public int PageNumber { get; init; } = 1;

    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public EOrderStatus? Status { get; init; }

    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
}