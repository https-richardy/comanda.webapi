namespace Comanda.WebApi.Handlers;

public sealed class FetchOrdersHandler(
    IOrderRepository orderRepository,
    IHttpContextAccessor contextAccessor
) :
    IRequestHandler<FetchOrdersRequest, Response<PaginationHelper<FormattedOrder>>>
{
    #pragma warning disable CS8604
    public async Task<Response<PaginationHelper<FormattedOrder>>> Handle(
        FetchOrdersRequest request,
        CancellationToken cancellationToken
    )
    {
        var orders = await orderRepository.PagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize
        );

        var formattedOrders = orders
            .Select(order => (FormattedOrder)order)
            .ToList();

        var pagination = new PaginationHelper<FormattedOrder>(
            data: formattedOrders,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            httpContext: contextAccessor.HttpContext
        );

        return new Response<PaginationHelper<FormattedOrder>>(
            data: pagination,
            statusCode: StatusCodes.Status200OK,
            message: "orders successfully retrieved."
        );
    }
}