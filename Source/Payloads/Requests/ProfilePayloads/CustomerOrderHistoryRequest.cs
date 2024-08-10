namespace Comanda.WebApi.Payloads;

public sealed record CustomerOrderHistoryRequest :
    IRequest<Response<PaginationHelper<FormattedOrder>>>
{
    public int PageSize { get; init; } = 10;
    public int PageNumber { get; init; } = 1;
}