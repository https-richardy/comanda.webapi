namespace Comanda.WebApi.Handlers;

public sealed class RetrieveCartAndAddressHandler(
    ICartRepository cartRepository,
    IAddressRepository addressRepository
) : IRequestHandler<RetrieveCartAndAddressRequest, (Cart cart, Address address)>
{
    public async Task<(Cart cart, Address address)> Handle(
        RetrieveCartAndAddressRequest request,
        CancellationToken cancellationToken
    )
    {
        var cart = await cartRepository.RetrieveByIdAsync(request.CartId);
        var address = await addressRepository.RetrieveByIdAsync(request.ShippingAddressId);
 
        return (cart, address);
    }
}