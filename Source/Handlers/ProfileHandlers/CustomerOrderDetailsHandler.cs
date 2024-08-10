namespace Comanda.WebApi.Handlers;

public sealed class CustomerOrderDetailsHandler(
    IOrderRepository orderRepository,
    IUserContextService userContextService,
    ICustomerRepository customerRepository
) : IRequestHandler<CustomerOrderDetailsRequest, Response<FormattedOrderDetails>>
{
    public async Task<Response<FormattedOrderDetails>> Handle(
        CustomerOrderDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        /*
            It is impossible for the 'userIdentifier' to be null at this point
            since the 'ProfileController' restricts access only to authenticated customers.
        */
        #pragma warning disable CS8604, CS8602

        var userIdentifier = userContextService.GetCurrentUserIdentifier();
        var customer = await customerRepository.FindCustomerByUserIdAsync(userIdentifier);

        var order = await orderRepository.RetrieveByIdAsync(request.OrderId);
        if (order is null)
            return new Response<FormattedOrderDetails>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: "order with the specified id was not found."
            );

        if (order.Customer.Id != customer.Id)
            return new Response<FormattedOrderDetails>(
                data: null,
                statusCode: StatusCodes.Status403Forbidden,
                message: "you do not have permission to access this order."
            );

        var formattedOrder = (FormattedOrderDetails)order;
        return new Response<FormattedOrderDetails>(
            data: formattedOrder,
            statusCode: StatusCodes.Status200OK,
            message: "order retrieved successfully."
        );
    }
}