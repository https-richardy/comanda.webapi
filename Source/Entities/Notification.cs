namespace Comanda.WebApi.Entities;

public sealed class Notification
{
    public string Title { get; set; }
    public string Message { get; set; }
    public DateTime Timestamp { get; set; }
}