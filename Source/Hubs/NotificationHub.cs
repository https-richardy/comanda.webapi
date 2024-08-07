namespace Comanda.WebApi.Hubs;

public sealed class NotificationHub : Hub
{
    public async Task SendNotificationAsync(Notification notification)
    {
        await Clients.All.SendAsync("receiveNotification", notification);
    }
}