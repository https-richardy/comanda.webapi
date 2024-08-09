namespace Comanda.WebApi.Handlers;

public sealed class OrderProcessingHandler(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    ISettingsRepository settingsRepository,
    IHubContext<NotificationHub> notificationHubContext
) : IRequestHandler<OrderProcessingRequest, Order>
{
    public async Task<Order> Handle(
        OrderProcessingRequest request,
        CancellationToken cancellationToken
    )
    {
        var settings = await settingsRepository.GetSettingsAsync();

        var orderStatus = EOrderStatus.Pending;
        if (settings.AcceptAutomatically)
        {
            var currentPendingOrders = await orderRepository.FindAllAsync(orders => orders.Status == EOrderStatus.Pending);
            if (currentPendingOrders.Count() < settings.MaxConcurrentAutomaticOrders)
            {
                orderStatus = EOrderStatus.Confirmed;
            }
        }

        var items = request.Cart.Items
            .Select(cartItem => (OrderItem)cartItem)
            .ToList();

        var order = new Order
        {
            Items = items,
            Customer = request.Cart.Customer,
            ShippingAddress = request.Address,
            Status = orderStatus,
            Date = DateTime.Now
        };

        await orderRepository.SaveAsync(order);
        await cartRepository.ClearCartAsync(request.Cart);

        await NotifyNewOrderAsync(order);

        return order;
    }

    private async Task NotifyNewOrderAsync(Order order)
    {
        var notification = new Notification
        {
            Title = "New Order!",
            Message = $"A new order has been created with ID: {order.Id}",
            Timestamp = DateTime.Now
        };

        await notificationHubContext.Clients.All.SendAsync("receiveNotification", notification);
    }
}