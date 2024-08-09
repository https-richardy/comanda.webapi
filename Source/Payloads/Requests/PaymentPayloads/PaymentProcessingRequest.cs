namespace Comanda.WebApi.Payloads;

public sealed record PaymentProcessingRequest : IRequest<Payment>
{
    public Session Session { get; init; }
    public Order Order { get; init; }
}