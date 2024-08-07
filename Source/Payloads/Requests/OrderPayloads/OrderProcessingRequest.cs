namespace Comanda.WebApi.Payloads;

public sealed record OrderProcessingRequest : IRequest<Order>
{
    public Cart Cart { get; init; }
    public Address Address { get; init; }
}