namespace Comanda.WebApi.Handlers;

public sealed class SetOrderStatusHandler(
    IOrderRepository orderRepository
) : IRequestHandler<SetOrderStatusRequest, Response>
{
    public async Task<Response> Handle(
        SetOrderStatusRequest request,
        CancellationToken token
    )
    {
        var order = await orderRepository.RetrieveByIdAsync(request.OrderId);
        if (order is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: $"order with ID '{request.OrderId}' was not found."
            );

        order.Status = request.Status;
        await orderRepository.UpdateAsync(order);

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "order status has been updated successfully."
        );
    }
}