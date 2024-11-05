namespace Comanda.WebApi.Hubs;

public sealed class NotificationHub : Hub
{
    public async Task SendNotificationAsync(Notification notification)
    {
        await Clients.All.SendAsync("receiveNotification", notification);
    }

    public async Task AssociateOrder(int orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderId.ToString());
        await Clients.Caller.SendAsync("associated", $"You are now associated with order {orderId}");
    }

    public async Task SendOrderStatusUpdateAsync(int orderId, Order updatedOrder)
    {
        await Clients
            .Group(orderId.ToString())
            .SendAsync("receiveOrderStatusUpdate", updatedOrder);
    }
}