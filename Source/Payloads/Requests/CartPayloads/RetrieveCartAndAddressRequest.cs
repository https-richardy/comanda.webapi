namespace Comanda.WebApi.Payloads;

public sealed record RetrieveCartAndAddressRequest : IRequest<(Cart cart, Address address)>
{
    public int CartId { get; init; }
    public int ShippingAddressId { get; init; }

    public RetrieveCartAndAddressRequest(int cartId, int shippingAddressId)
    {
        CartId = cartId;
        ShippingAddressId = shippingAddressId;
    }
}