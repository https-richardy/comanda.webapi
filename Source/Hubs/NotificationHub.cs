namespace Comanda.WebApi.Hubs;

public sealed class NotificationHub : Hub
{
    public async Task SendNotificationAsync(Notification notification)
    {
        await Clients.All.SendAsync("receiveNotification", notification);
    }

    public override async Task OnConnectedAsync()
    {
        var user = Context.User;
        if (user is null)
        {
            Context.Abort();
            return;
        }

        if (user.IsInRole("Administrator"))
            await Groups.AddToGroupAsync(Context.ConnectionId, "Administrators");

        await base.OnConnectedAsync();
    }
}