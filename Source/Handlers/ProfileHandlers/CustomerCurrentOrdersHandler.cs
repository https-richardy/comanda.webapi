namespace Comanda.WebApi.Handlers;

public sealed class CustomerCurrentOrdersHandler(
    IOrderRepository orderRepository,
    ICustomerRepository customerRepository,
    IUserContextService userContextService
) :
    IRequestHandler<CustomerCurrentOrdersRequest, Response<IEnumerable<FormattedOrder>>>
{
    public async Task<Response<IEnumerable<FormattedOrder>>> Handle(
        CustomerCurrentOrdersRequest request,
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

        Expression<Func<Order, bool>> predicate = order =>
            order.Customer.Id == customer.Id &&
            order.Status == EOrderStatus.Pending ||
            order.Status == EOrderStatus.InPreparation ||
            order.Status == EOrderStatus.Confirmed;

        var orders = await orderRepository.FindAllAsync(predicate: predicate);
        var formattedOrders = orders
            .Select(order => (FormattedOrder)order)
            .ToList();

        return new Response<IEnumerable<FormattedOrder>>(
            data: formattedOrders,
            statusCode: StatusCodes.Status200OK,
            message: "Orders retrieved successfully."
        );
    }
}