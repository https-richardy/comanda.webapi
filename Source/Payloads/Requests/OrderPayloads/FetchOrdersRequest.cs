namespace Comanda.WebApi.Payloads;

public sealed record FetchOrdersRequest :
    IRequest<Response<PaginationHelper<FormattedOrder>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public EOrderStatus StatusFilter = EOrderStatus.Pending;
}