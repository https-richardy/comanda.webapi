namespace Comanda.WebApi.Handlers;

public sealed class OrderProcessingHandler(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    ISettingsRepository settingsRepository
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
                orderStatus = EOrderStatus.Processing;
            }
        }

        var items = request.Cart.Items.Select(cartItem => new OrderItem
        {
            Quantity = cartItem.Quantity,
            Product = cartItem.Product,
            Additionals = cartItem.Additionals.Select(additional => new OrderItemAdditional
            {
                Additional = additional.Additional,
                Quantity = additional.Quantity
            }).ToList(),
            UnselectedIngredients = cartItem.UnselectedIngredients.Select(ingredient => new UnselectedIngredient
            {
                Ingredient = ingredient.Ingredient,
                CartItem = cartItem
            }).ToList()
        }).ToList();

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

        return order;
    }
}