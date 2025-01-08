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
        Expression<Func<Order, bool>> predicate = order =>
            (request.Status == null || order.Status == request.Status) &&
            order.Status != EOrderStatus.Delivered &&
            order.Status != EOrderStatus.CancelledByCustomer &&
            order.Status != EOrderStatus.CancelledBySystem;

        var orders = await orderRepository.PagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            predicate: predicate
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