namespace Comanda.WebApi.Handlers;

public sealed class FetchOrderHandler(IOrderRepository orderRepository) :
    IRequestHandler<OrderDetailsRequest, Response<FormattedOrderDetails>>
{
    public async Task<Response<FormattedOrderDetails>> Handle(
        OrderDetailsRequest request,
        CancellationToken cancellationToken
    )
    {
        var order = await orderRepository.RetrieveByIdAsync(request.OrderId);
        if (order is null)
            return new Response<FormattedOrderDetails>(
                data: null,
                statusCode: StatusCodes.Status404NotFound,
                message: $"order with ID '{request.OrderId}' was not found."
            );

        var formattedOrder = (FormattedOrderDetails)order;
        return new Response<FormattedOrderDetails>(
            data: formattedOrder,
            statusCode: StatusCodes.Status200OK,
            message: "the request was successfully recovered"
        );
    }
}