namespace Comanda.WebApi.Payloads;

public sealed record CheckoutResponse
{
    public string SessionId { get; init; }
    public string Url { get; init; }
}