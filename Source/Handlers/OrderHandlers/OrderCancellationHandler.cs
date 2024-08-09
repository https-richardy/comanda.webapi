namespace Comanda.WebApi.Handlers;

public sealed class OrderCancellationHandler(
    IOrderRepository orderRepository,
    IRefundManager refundManager,
    UserManager<Account> userManager,
    IUserContextService userContextService,
    ILogger<OrderCancellationHandler> logger
) : IRequestHandler<OrderCancellationRequest, Response>
{
    #pragma warning disable CS8604
    public async Task<Response> Handle(
        OrderCancellationRequest request,
        CancellationToken cancellationToken
    )
    {
        var userIdentifier = userContextService.GetCurrentUserIdentifier();
        var user = await userManager.FindByIdAsync(userIdentifier);

        var order = await orderRepository.RetrieveByIdAsync(request.OrderId);
        if (order is null)
            return new Response(
                statusCode: StatusCodes.Status404NotFound,
                message: "the order with the specified id was not found."
            );

        if (order.Status == EOrderStatus.Delivered || order.Status == EOrderStatus.Shipped)
        {
            return new Response(
                statusCode: StatusCodes.Status400BadRequest,
                message: "The order cannot be cancelled as it has already been delivered or shipped."
            );
        }

        if (!await userManager.IsInRoleAsync(user, "Administrator"))
        {
            if (order.Customer.Account.Id != user.Id)
            {
                return new Response(
                    statusCode: StatusCodes.Status403Forbidden,
                    message: "You are not authorized to cancel this order."
                );
            }

            order.Status = EOrderStatus.CancelledByCustomer;
        }
        else
            order.Status = EOrderStatus.CancelledBySystem;


        if (!string.IsNullOrEmpty(request.Reason))
            order.CancelledReason = request.Reason;

        await orderRepository.UpdateAsync(order);

        try
        {
            await refundManager.RefundOrderAsync(order);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to process refund for Order ID {orderId}.", order.Id);
            return new Response(
                statusCode: StatusCodes.Status500InternalServerError,
                message: "An error occurred while processing the refund."
            );
        }

        return new Response(
            statusCode: StatusCodes.Status200OK,
            message: "The order has been successfully cancelled."
        );
    }
}