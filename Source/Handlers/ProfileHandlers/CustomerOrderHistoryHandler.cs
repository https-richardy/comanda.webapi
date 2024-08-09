namespace Comanda.WebApi.Handlers;

public sealed class CustomerOrdersHistoryHandler(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService,
    IHttpContextAccessor contextAccessor
) : IRequestHandler<CustomerOrderHistoryRequest, Response<PaginationHelper<FormattedOrder>>>
{
    public async Task<Response<PaginationHelper<FormattedOrder>>> Handle(
        CustomerOrderHistoryRequest request,
        CancellationToken cancellationToken
    )
    {
        /*
            It is impossible for the 'userIdentifier' to be null at this point
            since the 'ProfileController' restricts access only to authenticated customers.
        */
        #pragma warning disable CS8604, CS8602

        var userId = userContextService.GetCurrentUserIdentifier();
        var customer = await customerRepository.FindCustomerByUserIdAsync(userId);

        Expression<Func<Order, bool>> predicate = order => order.Customer.Id == customer.Id;

        if (request.StartDate.HasValue)
            predicate = predicate.And(order => order.Date >= request.StartDate.Value);

        if (request.EndDate.HasValue)
            predicate = predicate.And(order => order.Date <= request.EndDate.Value.Date.AddDays(1).AddTicks(-1));

        if (request.Status.HasValue)
            predicate = predicate.And(order => order.Status == request.Status.Value);

        if (request.MinPrice.HasValue)
            predicate = predicate.And(order => order.Total >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            predicate = predicate.And(order => order.Total <= request.MaxPrice.Value);

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
            httpContext: contextAccessor?.HttpContext
        );

        return new Response<PaginationHelper<FormattedOrder>>(
            data: pagination,
            statusCode: StatusCodes.Status200OK,
            message: "Customer orders history retrieved successfully.");
    }
}