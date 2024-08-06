namespace Comanda.WebApi.Payloads;

public sealed record CheckoutRequest : IRequest<Response<CheckoutResponse>>
{
    public int ShippingAddressId { get; init; }
}